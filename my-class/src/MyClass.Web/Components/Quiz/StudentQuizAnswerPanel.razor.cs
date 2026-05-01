using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MyClass.Core.Models;
using MyClass.Web.Hubs;

namespace MyClass.Web.Components.Quiz;

public partial class StudentQuizAnswerPanel
{
    [Parameter, EditorRequired]
    public ClassContext CurrentClass { get; set; } = null!;

    private Result<QuizAnswerPageState>? _stateResult;
    private HubConnection? _hubConnection;
    private LoginState? _loginState;
    private CancellationTokenSource? _pollingCancellation;
    private CancellationTokenSource? _timerCancellation;
    private int? _loadedClassId;
    private int? _pendingClassId;
    private bool _isLoading = true;
    private bool _isSubmitting;
    private bool _lastSubmitSucceeded;
    private string? _lastSubmitMessage;
    private string? _loadedImageQuestionKey;
    private bool? _loadedAnswerRevealState;
    private string? _imageDataUri;
    private string? _imageMessage;
    private DateTime? _timerEndsAtUtc;
    private TimeSpan _timerRemaining = TimeSpan.Zero;
    private bool _isTimerRunning;

    private IReadOnlyList<string> AnswerChoices => _stateResult?.Value?.AnswerChoices ?? [];

    private bool DisableAnswerButtons =>
        _isSubmitting ||
        _stateResult?.Value?.HasInProgressAnswer != true ||
        _stateResult?.Value?.IsAnswerRevealed == true;

    private string QuestionImageAltText =>
        _stateResult?.Value?.IsAnswerRevealed == true
            ? "Current quiz answer"
            : _stateResult?.Value?.QuestionTitle ?? "Current quiz question";

    private string QuizTitle => _stateResult?.Value?.QuizTitle ?? "Quiz Answer";

    private string? QuestionPositionText
    {
        get
        {
            var state = _stateResult?.Value;

            if (state?.QuestionIndex is null || state.QuestionCount is null)
            {
                return null;
            }

            return $"Question {state.QuestionIndex.Value + 1} of {state.QuestionCount.Value}";
        }
    }

    private bool ShowTimer =>
        !string.IsNullOrWhiteSpace(_stateResult?.Value?.QuestionKey) &&
        _stateResult?.Value?.IsAnswerRevealed != true;

    private string? RevealMessage => _stateResult?.Value?.RevealMessage;

    private bool IsTimerRunning => _isTimerRunning;

    private TimeSpan TimerRemaining => _timerRemaining;

    private string StatusMessage
    {
        get
        {
            if (_stateResult?.Value is null)
            {
                return "Waiting for quiz state.";
            }

            return _stateResult.Value.Message;
        }
    }

    protected override void OnParametersSet()
    {
        if (_loadedClassId == CurrentClass.ClassId)
        {
            return;
        }

        _pendingClassId = CurrentClass.ClassId;
        _isLoading = true;
        _stateResult = null;
        _lastSubmitMessage = null;
        ResetImageState();
        ResetTimerState();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_pendingClassId != CurrentClass.ClassId)
        {
            return;
        }

        _pendingClassId = null;
        await LoadStateAsync(showLoading: true);
        await StartSignalRAsync();
        StartPolling();
        StateHasChanged();
    }

    private async Task LoadStateAsync(bool showLoading)
    {
        if (showLoading)
        {
            _isLoading = true;
        }

        _loadedClassId = CurrentClass.ClassId;
        _loginState = LoginStateService.Current ?? await SessionStorage.GetLoginStateAsync();
        LoginStateService.Set(_loginState);

        _stateResult = await QuizAnswerService.GetAnswerPageStateAsync(_loginState, CurrentClass);
        await LoadCurrentImageAsync();
        UpdateTimerState();
        _isLoading = false;
    }

    private async Task LoadCurrentImageAsync()
    {
        var questionKey = _stateResult?.Value?.QuestionKey;

        if (string.IsNullOrWhiteSpace(questionKey))
        {
            ResetImageState();
            return;
        }

        if (string.Equals(questionKey, _loadedImageQuestionKey, StringComparison.Ordinal))
        {
            if (_stateResult?.Value?.IsAnswerRevealed == _loadedAnswerRevealState)
            {
                return;
            }
        }

        _loadedImageQuestionKey = questionKey;
        _loadedAnswerRevealState = _stateResult?.Value?.IsAnswerRevealed == true;
        _imageDataUri = null;
        _imageMessage = null;

        var imageResult = _loadedAnswerRevealState == true
            ? await QuizContentService.LoadAnswerImageAsync(questionKey)
            : await QuizContentService.LoadQuestionImageAsync(questionKey);

        if (!imageResult.Succeeded)
        {
            _imageMessage = imageResult.Message;
            return;
        }

        _imageDataUri = imageResult.Value;
    }

    private void ResetImageState()
    {
        _loadedImageQuestionKey = null;
        _loadedAnswerRevealState = null;
        _imageDataUri = null;
        _imageMessage = null;
    }

    private void UpdateTimerState()
    {
        var state = _stateResult?.Value;

        if (state is null || string.IsNullOrWhiteSpace(state.QuestionKey))
        {
            ResetTimerState();
            return;
        }

        _timerRemaining = state.CurrentQuestionRemaining < TimeSpan.Zero
            ? TimeSpan.Zero
            : state.CurrentQuestionRemaining;
        _isTimerRunning = !state.IsAnswerRevealed && state.CurrentQuestionIsInProgress && _timerRemaining > TimeSpan.Zero;
        _timerEndsAtUtc = _isTimerRunning ? DateTime.UtcNow.Add(_timerRemaining) : null;

        if (_isTimerRunning)
        {
            StartTimer();
            return;
        }

        StopTimer();
    }

    private void ResetTimerState()
    {
        StopTimer();
        _timerEndsAtUtc = null;
        _timerRemaining = TimeSpan.Zero;
        _isTimerRunning = false;
    }

    private void StartTimer()
    {
        if (_timerCancellation is not null)
        {
            return;
        }

        _timerCancellation = new CancellationTokenSource();
        _ = TimerAsync(_timerCancellation.Token);
    }

    private void StopTimer()
    {
        _timerCancellation?.Cancel();
        _timerCancellation?.Dispose();
        _timerCancellation = null;
    }

    private async Task TimerAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await InvokeAsync(() =>
                {
                    UpdateLocalTimerRemaining();
                    StateHasChanged();
                });
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void UpdateLocalTimerRemaining()
    {
        if (_timerEndsAtUtc is null)
        {
            _timerRemaining = TimeSpan.Zero;
            _isTimerRunning = false;
            StopTimer();
            return;
        }

        var remaining = _timerEndsAtUtc.Value - DateTime.UtcNow;

        if (remaining <= TimeSpan.Zero)
        {
            _timerRemaining = TimeSpan.Zero;
            _isTimerRunning = false;
            _timerEndsAtUtc = null;
            StopTimer();
            return;
        }

        _timerRemaining = remaining;
    }

    private void StartPolling()
    {
        _pollingCancellation?.Cancel();
        _pollingCancellation?.Dispose();
        _pollingCancellation = new CancellationTokenSource();
        _ = PollAsync(_pollingCancellation.Token);
    }

    private async Task StartSignalRAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }

        if (string.IsNullOrWhiteSpace(CurrentClass.Code))
        {
            return;
        }

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri(QuizHub.Route))
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On(QuizHub.QuizStateChangedMethod, () =>
        {
            _ = InvokeAsync(async () =>
            {
                await LoadStateAsync(showLoading: false);
                StateHasChanged();
            });
        });

        _hubConnection.Reconnected += async _ =>
        {
            await JoinCurrentClassGroupAsync();
            await InvokeAsync(async () =>
            {
                await LoadStateAsync(showLoading: false);
                StateHasChanged();
            });
        };

        await _hubConnection.StartAsync();
        await JoinCurrentClassGroupAsync();
    }

    private async Task JoinCurrentClassGroupAsync()
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("JoinClass", CurrentClass.Code);
        }
    }

    private async Task PollAsync(CancellationToken cancellationToken)
    {
        var interval = Math.Max(500, QuizOptions.Value.StatusRefreshMilliseconds);
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(interval));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await InvokeAsync(async () =>
                {
                    await LoadStateAsync(showLoading: false);
                    StateHasChanged();
                });
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task SubmitAnswerAsync(string selectedAnswer)
    {
        if (_isSubmitting)
        {
            return;
        }

        try
        {
            _isSubmitting = true;

            var result = await QuizAnswerService.SubmitAnswerAsync(_loginState, CurrentClass, selectedAnswer);
            _lastSubmitSucceeded = result.Succeeded;
            _lastSubmitMessage = result.Message;

            await LoadStateAsync(showLoading: false);
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private static string FormatRemaining(TimeSpan remaining)
    {
        return $"{(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";
    }

    public async ValueTask DisposeAsync()
    {
        _pollingCancellation?.Cancel();
        _pollingCancellation?.Dispose();
        StopTimer();

        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
