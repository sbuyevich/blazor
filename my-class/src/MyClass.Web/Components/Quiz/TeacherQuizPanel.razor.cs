using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyClass.Core.Models;

namespace MyClass.Web.Components.Quiz;

public partial class TeacherQuizPanel
{
    [Parameter, EditorRequired]
    public ClassContext CurrentClass { get; set; } = null!;

    private Result<QuizTeacherState>? _stateResult;
    private Result<IReadOnlyList<QuizSummary>>? _availableQuizzesResult;
    private IReadOnlyList<QuizSummary> _availableQuizzes = [];
    private LoginState? _loginState;
    private CancellationTokenSource? _pollingCancellation;
    private int? _loadedClassId;
    private int? _pendingClassId;
    private string? _selectedQuizPath;
    private string? _loadedImageQuestionKey;
    private bool? _loadedAnswerRevealState;
    private string? _imageDataUri;
    private string? _imageMessage;
    private bool _isLoading = true;
    private bool _isWorking;
    private bool _quizSelectionLoaded;
    private bool HasSelectedQuiz => !string.IsNullOrWhiteSpace(_selectedQuizPath);

    protected override void OnParametersSet()
    {
        if (_loadedClassId == CurrentClass.ClassId)
        {
            return;
        }

        _pendingClassId = CurrentClass.ClassId;
        _isLoading = true;
        _stateResult = null;
        _availableQuizzesResult = null;
        _availableQuizzes = [];
        _selectedQuizPath = null;
        _quizSelectionLoaded = false;
        _loadedImageQuestionKey = null;
        _loadedAnswerRevealState = null;
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

        await LoadQuizSelectionAsync();

        _stateResult = await QuizSessionService.GetTeacherStateAsync(_loginState, CurrentClass, _selectedQuizPath);
        await LoadCurrentImageAsync();
        _isLoading = false;
    }

    private async Task LoadQuizSelectionAsync()
    {
        if (_quizSelectionLoaded)
        {
            return;
        }

        _availableQuizzesResult = await QuizContentService.LoadAvailableQuizzesAsync();
        _availableQuizzes = _availableQuizzesResult.Succeeded && _availableQuizzesResult.Value is not null
            ? _availableQuizzesResult.Value
            : [];

        var savedQuizPath = await SessionStorage.GetSelectedQuizPathAsync();
        var selectedQuiz = _availableQuizzes.FirstOrDefault(quiz =>
                string.Equals(quiz.Path, savedQuizPath, StringComparison.OrdinalIgnoreCase)) ??
            _availableQuizzes.FirstOrDefault();

        _selectedQuizPath = selectedQuiz?.Path;

        if (string.IsNullOrWhiteSpace(_selectedQuizPath))
        {
            await SessionStorage.RemoveSelectedQuizPathAsync();
        }
        else if (!string.Equals(_selectedQuizPath, savedQuizPath, StringComparison.OrdinalIgnoreCase))
        {
            await SessionStorage.SetSelectedQuizPathAsync(_selectedQuizPath);
        }

        _quizSelectionLoaded = true;
    }

    private async Task LoadCurrentImageAsync()
    {
        var question = _stateResult?.Value?.CurrentQuestion;

        if (question is null)
        {
            _loadedImageQuestionKey = null;
            _loadedAnswerRevealState = null;
            _imageDataUri = null;
            _imageMessage = null;
            return;
        }

        if (question.QuestionKey == _loadedImageQuestionKey &&
            question.IsAnswerRevealed == _loadedAnswerRevealState)
        {
            return;
        }

        _loadedImageQuestionKey = question.QuestionKey;
        _loadedAnswerRevealState = question.IsAnswerRevealed;
        _imageDataUri = null;
        _imageMessage = null;

        var imageResult = question.IsAnswerRevealed
            ? await QuizContentService.LoadAnswerImageAsync(question.QuestionKey, _selectedQuizPath)
            : await QuizContentService.LoadQuestionImageAsync(question.QuestionKey, _selectedQuizPath);

        if (!imageResult.Succeeded)
        {
            _imageMessage = imageResult.Message;
            return;
        }

        _imageDataUri = imageResult.Value;
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
        if (_stateResult?.Value?.CurrentQuestion?.IsInProgress == true)
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
        _loadedAnswerRevealState = null;
        await RunActionAsync(() => QuizSessionService.StartQuestionAsync(_loginState, CurrentClass, _selectedQuizPath));
    }

    private async Task RestartQuizAsync()
    {
        _loadedImageQuestionKey = null;
        _loadedAnswerRevealState = null;
        await RunActionAsync(() => QuizSessionService.RestartQuizAsync(_loginState, CurrentClass, _selectedQuizPath));
    }

    private async Task FinishQuestionAsync()
    {
        await FinishCurrentQuestionWithoutReloadAsync();
    }

    private async Task MoveNextQuestionAsync()
    {
        _loadedImageQuestionKey = null;
        _loadedAnswerRevealState = null;
        await RunActionAsync(() => QuizSessionService.MoveNextQuestionAsync(_loginState, CurrentClass, _selectedQuizPath));
    }

    private async Task ShowAnswerAsync()
    {
        _loadedAnswerRevealState = null;
        await RunActionAsync(() => QuizSessionService.ShowAnswerAsync(_loginState, CurrentClass, _selectedQuizPath));
    }

    private async Task RunActionAsync(Func<Task<Result<bool>>> action)
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
        var result = await QuizSessionService.FinishCurrentQuestionAsync(_loginState, CurrentClass, _selectedQuizPath);

        if (result.Succeeded)
        {
            ApplyCurrentQuestionFinishedState();
        }

        _isWorking = false;
    }

    private void ApplyCurrentQuestionFinishedState()
    {
        if (_stateResult?.Value?.CurrentQuestion is null)
        {
            return;
        }

        var state = _stateResult.Value;
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

        _stateResult = Result<QuizTeacherState>.Success(state with
        {
            IsComplete = false,
            CurrentQuestion = finishedQuestion,
            Students = students
        });
    }

    private bool IsCurrentQuestionTimerExpired()
    {
        var question = _stateResult?.Value?.CurrentQuestion;

        if (question is null || !question.IsInProgress)
        {
            return false;
        }

        return DateTime.UtcNow >= question.StartedAtUtc.AddSeconds(question.TimeoutSeconds);
    }

    private async Task HandleQuizSelectedAsync(string quizPath)
    {
        _selectedQuizPath = string.IsNullOrWhiteSpace(quizPath) ? null : quizPath;
        _loadedImageQuestionKey = null;
        _loadedAnswerRevealState = null;
        _imageDataUri = null;
        _imageMessage = null;

        if (string.IsNullOrWhiteSpace(_selectedQuizPath))
        {
            await SessionStorage.RemoveSelectedQuizPathAsync();
        }
        else
        {
            await SessionStorage.SetSelectedQuizPathAsync(_selectedQuizPath);
        }

        await LoadStateAsync(showLoading: false);
        StartPollingIfNeeded();
    }

    private static string FormatRemaining(TimeSpan remaining)
    {
        return $"{(int)remaining.TotalMinutes:00}:{remaining.Seconds:00}";
    }

    private static bool IsLastQuestion(QuizTeacherQuestionState? question)
    {
        return question is not null && question.QuestionIndex >= question.QuestionCount - 1;
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

    private static int GetStatusSortValue(QuizStudentAnswerStatus status)
    {
        if (status.HasAnswered)
        {
            return 0;
        }

        return status.FailedNoAnswer ? 2 : 1;
    }

    private static string FormatAnswerElapsed(QuizStudentAnswerStatus status)
    {
        return status.AnswerElapsed is null
            ? "-"
            : $"{(int)status.AnswerElapsed.Value.TotalSeconds}s";
    }

    private static string GetCorrectText(QuizStudentAnswerStatus status)
    {
        return status.IsCorrect switch
        {
            true => "Correct",
            false => "Incorrect",
            _ => "-"
        };
    }

    private static int GetCorrectSortValue(QuizStudentAnswerStatus status)
    {
        return status.IsCorrect switch
        {
            true => 0,
            false => 1,
            _ => 2
        };
    }

    private static Color GetCorrectColor(QuizStudentAnswerStatus status)
    {
        return status.IsCorrect switch
        {
            true => Color.Success,
            false => Color.Error,
            _ => Color.Default
        };
    }

    public void Dispose()
    {
        StopPolling();
    }
}
