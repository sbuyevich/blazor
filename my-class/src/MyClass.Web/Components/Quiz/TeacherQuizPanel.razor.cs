using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyClass.Core.Models;

namespace MyClass.Web.Components.Quiz;

public partial class TeacherQuizPanel
{
    [Parameter, EditorRequired]
    public ClassContext CurrentClass { get; set; } = null!;

    private QuizTeacherStateResult? _stateResult;
    private LoginState? _loginState;
    private CancellationTokenSource? _pollingCancellation;
    private int? _loadedClassId;
    private int? _pendingClassId;
    private string? _loadedImageQuestionKey;
    private string? _imageDataUri;
    private string? _imageMessage;
    private bool _isLoading = true;
    private bool _isWorking;

    protected override void OnParametersSet()
    {
        if (_loadedClassId == CurrentClass.ClassId)
        {
            return;
        }

        _pendingClassId = CurrentClass.ClassId;
        _isLoading = true;
        _stateResult = null;
        _loadedImageQuestionKey = null;
        _imageDataUri = null;
        _imageMessage = null;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_pendingClassId != CurrentClass.ClassId)
        {
            return;
        }

        _pendingClassId = null;
        await LoadStateAsync(showLoading: true);
        StartPollingIfNeeded();
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

        _stateResult = await QuizSessionService.GetTeacherStateAsync(_loginState, CurrentClass);
        await LoadCurrentImageAsync();
        _isLoading = false;
    }

    private async Task LoadCurrentImageAsync()
    {
        var question = _stateResult?.State?.CurrentQuestion;

        if (question is null || question.QuestionKey == _loadedImageQuestionKey)
        {
            return;
        }

        _loadedImageQuestionKey = question.QuestionKey;
        _imageDataUri = null;
        _imageMessage = null;

        var imageResult = await QuizContentService.LoadQuestionImageAsync(question.QuestionKey);

        if (!imageResult.Succeeded)
        {
            _imageMessage = imageResult.Message;
            return;
        }

        _imageDataUri = imageResult.DataUri;
    }

    private void StartPolling()
    {
        _pollingCancellation?.Cancel();
        _pollingCancellation?.Dispose();
        _pollingCancellation = new CancellationTokenSource();
        _ = PollAsync(_pollingCancellation.Token);
    }

    private void StartPollingIfNeeded()
    {
        if (_stateResult?.State?.CurrentQuestion?.IsInProgress == true)
        {
            StartPolling();
            return;
        }

        StopPolling();
    }

    private void StopPolling()
    {
        _pollingCancellation?.Cancel();
        _pollingCancellation?.Dispose();
        _pollingCancellation = null;
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
                    if (IsCurrentQuestionTimerExpired())
                    {
                        await FinishCurrentQuestionWithoutReloadAsync();
                        StateHasChanged();
                        return;
                    }

                    await LoadStateAsync(showLoading: false);
                    StartPollingIfNeeded();
                    StateHasChanged();
                });
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task StartQuestionAsync()
    {
        _loadedImageQuestionKey = null;
        await RunActionAsync(() => QuizSessionService.StartQuestionAsync(_loginState, CurrentClass));
    }

    private async Task RestartQuizAsync()
    {
        _loadedImageQuestionKey = null;
        await RunActionAsync(() => QuizSessionService.RestartQuizAsync(_loginState, CurrentClass));
    }

    private async Task FinishQuestionAsync()
    {
        await FinishCurrentQuestionWithoutReloadAsync();
    }

    private async Task MoveNextQuestionAsync()
    {
        _loadedImageQuestionKey = null;
        await RunActionAsync(() => QuizSessionService.MoveNextQuestionAsync(_loginState, CurrentClass));
    }

    private async Task RunActionAsync(Func<Task<QuizActionResult>> action)
    {
        _isWorking = true;
        var result = await action();
        await LoadStateAsync(showLoading: false);
        StartPollingIfNeeded();
        _isWorking = false;
    }

    private async Task FinishCurrentQuestionWithoutReloadAsync()
    {
        StopPolling();

        _isWorking = true;
        var result = await QuizSessionService.FinishCurrentQuestionAsync(_loginState, CurrentClass);

        if (result.Succeeded)
        {
            ApplyCurrentQuestionFinishedState();
        }

        _isWorking = false;
    }

    private void ApplyCurrentQuestionFinishedState()
    {
        if (_stateResult?.State?.CurrentQuestion is null)
        {
            return;
        }

        var state = _stateResult.State;
        var question = state.CurrentQuestion;
        var finishedQuestion = question with
        {
            FinishedAtUtc = DateTime.UtcNow,
            IsInProgress = false,
            Remaining = TimeSpan.Zero
        };

        var students = state.Students
            .Select(student => student.HasAnswered ? student : student with { FailedNoAnswer = true })
            .ToList();

        _stateResult = QuizTeacherStateResult.Success(state with
        {
            IsComplete = question.QuestionIndex >= question.QuestionCount - 1,
            CurrentQuestion = finishedQuestion,
            Students = students
        });
    }

    private bool IsCurrentQuestionTimerExpired()
    {
        var question = _stateResult?.State?.CurrentQuestion;

        if (question is null || !question.IsInProgress)
        {
            return false;
        }

        return DateTime.UtcNow >= question.StartedAtUtc.AddSeconds(question.TimeoutSeconds);
    }

    private static string FormatRemaining(TimeSpan remaining)
    {
        return $"{(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";
    }

    private static Color GetStatusColor(QuizStudentAnswerStatus status)
    {
        if (status.HasAnswered)
        {
            return Color.Success;
        }

        return status.FailedNoAnswer ? Color.Error : Color.Default;
    }

    private static string GetStatusText(QuizStudentAnswerStatus status)
    {
        if (status.HasAnswered)
        {
            return "Answered";
        }

        return "Not answered";
    }

    public void Dispose()
    {
        StopPolling();
    }
}