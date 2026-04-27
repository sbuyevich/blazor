using Microsoft.AspNetCore.Components;
using MyClass.Core.Services.Auth;
using MyClass.Core.Services.ClassContext;
using MyClass.Core.Services.Quiz;

namespace MyClass.Web.Components.Quiz;

public partial class StudentQuizAnswerPanel
{
    private static readonly int[] AnswerChoices = [1, 2, 3, 4];

    [Parameter, EditorRequired]
    public ClassContext CurrentClass { get; set; } = null!;

    private QuizAnswerPageStateResult? _stateResult;
    private LoginState? _loginState;
    private CancellationTokenSource? _pollingCancellation;
    private int? _loadedClassId;
    private int? _pendingClassId;
    private bool _isLoading = true;
    private bool _isSubmitting;
    private bool _lastSubmitSucceeded;
    private string? _lastSubmitMessage;

    private string StatusMessage
    {
        get
        {
            if (_stateResult?.State is null)
            {
                return "Waiting for quiz state.";
            }

            return _stateResult.State.Message;
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
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_pendingClassId != CurrentClass.ClassId)
        {
            return;
        }

        _pendingClassId = null;
        await LoadStateAsync(showLoading: true);
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
        _isLoading = false;
    }

    private void StartPolling()
    {
        _pollingCancellation?.Cancel();
        _pollingCancellation?.Dispose();
        _pollingCancellation = new CancellationTokenSource();
        _ = PollAsync(_pollingCancellation.Token);
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

    public void Dispose()
    {
        _pollingCancellation?.Cancel();
        _pollingCancellation?.Dispose();
    }
}