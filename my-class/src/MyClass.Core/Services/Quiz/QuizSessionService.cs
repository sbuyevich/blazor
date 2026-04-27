using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Data;
using MyClass.Data.Entities;
using MyClass.Options;
using MyClass.Services.Auth;
using ClassContextModel = MyClass.Services.ClassContext.ClassContext;

namespace MyClass.Services.Quiz;

public sealed class QuizSessionService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IQuizContentService quizContentService,
    IOptions<TeacherOptions> teacherOptions) : IQuizSessionService
{
    public async Task<QuizTeacherStateResult> GetTeacherStateAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState, currentClass);

        if (authorizationMessage is not null)
        {
            return QuizTeacherStateResult.Failure(authorizationMessage);
        }

        var contentResult = await quizContentService.LoadQuizAsync(cancellationToken);

        if (!contentResult.Succeeded || contentResult.Quiz is null)
        {
            return QuizTeacherStateResult.Failure(contentResult.Message);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var currentQuestion = await GetCurrentLiveQuestionAsync(dbContext, contentResult.Quiz, now, cancellationToken);

        if (currentQuestion is not null && currentQuestion.HasOpenAnswers && currentQuestion.IsExpired)
        {
            await FinishQuestionRowsAsync(dbContext, currentQuestion, now, cancellationToken);
            currentQuestion = await GetCurrentLiveQuestionAsync(dbContext, contentResult.Quiz, now, cancellationToken);
        }

        var state = await BuildTeacherStateAsync(
            dbContext,
            currentClass.ClassId,
            contentResult.Quiz,
            currentQuestion,
            cancellationToken);

        return QuizTeacherStateResult.Success(state);
    }

    public async Task<QuizActionResult> StartQuestionAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default)
    {
        return await StartOrRestartQuizAsync(loginState, currentClass, "Quiz started.", cancellationToken);
    }

    public async Task<QuizActionResult> RestartQuizAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default)
    {
        return await StartOrRestartQuizAsync(loginState, currentClass, "Quiz restarted.", cancellationToken);
    }

    private async Task<QuizActionResult> StartOrRestartQuizAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        string successMessage,
        CancellationToken cancellationToken)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState, currentClass);

        if (authorizationMessage is not null)
        {
            return QuizActionResult.Failure(authorizationMessage);
        }

        var contentResult = await quizContentService.LoadQuizAsync(cancellationToken);

        if (!contentResult.Succeeded || contentResult.Quiz is null)
        {
            return QuizActionResult.Failure(contentResult.Message);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.QuizAnswers.ExecuteDeleteAsync(cancellationToken);

        var createdCount = await CreateQuestionRowsAsync(
            dbContext,
            currentClass.ClassId,
            contentResult.Quiz.Questions[0],
            DateTime.UtcNow,
            cancellationToken);

        if (createdCount == 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return QuizActionResult.Failure("No active students are available for this quiz.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return QuizActionResult.Success(successMessage);
    }

    public async Task<QuizActionResult> FinishCurrentQuestionAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState, currentClass);

        if (authorizationMessage is not null)
        {
            return QuizActionResult.Failure(authorizationMessage);
        }

        var contentResult = await quizContentService.LoadQuizAsync(cancellationToken);

        if (!contentResult.Succeeded || contentResult.Quiz is null)
        {
            return QuizActionResult.Failure(contentResult.Message);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var currentQuestion = await GetCurrentLiveQuestionAsync(dbContext, contentResult.Quiz, DateTime.UtcNow, cancellationToken);

        if (currentQuestion is null)
        {
            return QuizActionResult.Failure("No question is in progress.");
        }

        if (!currentQuestion.HasOpenAnswers)
        {
            return QuizActionResult.Success("Question is already finished.");
        }

        await FinishQuestionRowsAsync(dbContext, currentQuestion, DateTime.UtcNow, cancellationToken);

        return QuizActionResult.Success("Question finished.");
    }

    public async Task<QuizActionResult> MoveNextQuestionAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState, currentClass);

        if (authorizationMessage is not null)
        {
            return QuizActionResult.Failure(authorizationMessage);
        }

        var contentResult = await quizContentService.LoadQuizAsync(cancellationToken);

        if (!contentResult.Succeeded || contentResult.Quiz is null)
        {
            return QuizActionResult.Failure(contentResult.Message);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var currentQuestion = await GetCurrentLiveQuestionAsync(dbContext, contentResult.Quiz, now, cancellationToken);

        if (currentQuestion is null)
        {
            return QuizActionResult.Failure("No quiz session is in progress.");
        }

        if (currentQuestion.HasOpenAnswers && currentQuestion.IsExpired)
        {
            await FinishQuestionRowsAsync(dbContext, currentQuestion, now, cancellationToken);
            currentQuestion = await GetCurrentLiveQuestionAsync(dbContext, contentResult.Quiz, now, cancellationToken);
        }

        if (currentQuestion?.IsInProgress == true)
        {
            return QuizActionResult.Failure("Finish the current question before moving next.");
        }

        var nextIndex = currentQuestion!.QuestionIndex + 1;

        if (nextIndex >= contentResult.Quiz.Questions.Count)
        {
            return QuizActionResult.Success("Quiz complete.");
        }

        var createdCount = await CreateQuestionRowsAsync(
            dbContext,
            currentClass.ClassId,
            contentResult.Quiz.Questions[nextIndex],
            now,
            cancellationToken);

        if (createdCount == 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return QuizActionResult.Failure("No active students are available for the next question.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return QuizActionResult.Success("Next question started.");
    }

    private static async Task<int> CreateQuestionRowsAsync(
        ApplicationDbContext dbContext,
        int classId,
        QuizQuestionContent questionContent,
        DateTime startedAtUtc,
        CancellationToken cancellationToken)
    {
        var activeStudents = await dbContext.Students
            .AsNoTracking()
            .Where(student => student.ClassId == classId && student.IsActive)
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ThenBy(student => student.UserName)
            .ToListAsync(cancellationToken);

        dbContext.QuizAnswers.AddRange(activeStudents.Select(student => new QuizAnswer
        {
            StudentId = student.Id,
            StudentUserName = student.UserName,
            StudentFirstName = student.FirstName,
            StudentLastName = student.LastName,
            StudentDisplayName = student.DisplayName,
            QuestionKey = questionContent.Key,
            QuestionIndex = questionContent.Index,
            QuestionText = questionContent.Title,
            CorrectAnswer = questionContent.CorrectAnswer,
            Answer = string.Empty,
            StartedAtUtc = startedAtUtc,
            IsCorrect = false
        }));

        return activeStudents.Count;
    }

    private static async Task FinishQuestionRowsAsync(
        ApplicationDbContext dbContext,
        LiveQuestionState question,
        DateTime endedAtUtc,
        CancellationToken cancellationToken)
    {
        var answers = await dbContext.QuizAnswers
            .Where(answer =>
                answer.QuestionIndex == question.QuestionIndex &&
                answer.QuestionKey == question.QuestionKey)
            .ToListAsync(cancellationToken);

        foreach (var answer in answers)
        {
            answer.EndedAtUtc ??= endedAtUtc;
            answer.IsCorrect = answer.Answer.Length > 0 &&
                string.Equals(answer.Answer, answer.CorrectAnswer, StringComparison.Ordinal);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<LiveQuestionState?> GetCurrentLiveQuestionAsync(
        ApplicationDbContext dbContext,
        QuizContent quiz,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var latestQuestion = await dbContext.QuizAnswers
            .AsNoTracking()
            .OrderByDescending(answer => answer.QuestionIndex)
            .ThenByDescending(answer => answer.StartedAtUtc)
            .Select(answer => new
            {
                answer.QuestionIndex,
                answer.QuestionKey,
                answer.QuestionText
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (latestQuestion is null)
        {
            return null;
        }

        var rows = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer =>
                answer.QuestionIndex == latestQuestion.QuestionIndex &&
                answer.QuestionKey == latestQuestion.QuestionKey)
            .Select(answer => new
            {
                answer.StartedAtUtc,
                answer.EndedAtUtc
            })
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
        {
            return null;
        }

        var questionContent = quiz.Questions.FirstOrDefault(question =>
            question.Index == latestQuestion.QuestionIndex &&
            string.Equals(question.Key, latestQuestion.QuestionKey, StringComparison.OrdinalIgnoreCase));

        var startedAtUtc = rows.Min(row => row.StartedAtUtc);
        var timeoutSeconds = questionContent?.TimeoutSeconds ?? quiz.TimeLimitSeconds;
        var hasOpenAnswers = rows.Any(row => row.EndedAtUtc is null);
        var isExpired = now >= startedAtUtc.AddSeconds(timeoutSeconds);
        var isInProgress = hasOpenAnswers && !isExpired;
        var finishedAtUtc = hasOpenAnswers ? null : rows.Max(row => row.EndedAtUtc);
        var remaining = isInProgress
            ? startedAtUtc.AddSeconds(timeoutSeconds) - now
            : TimeSpan.Zero;

        return new LiveQuestionState(
            latestQuestion.QuestionIndex,
            latestQuestion.QuestionKey,
            questionContent?.Title ?? latestQuestion.QuestionText,
            timeoutSeconds,
            startedAtUtc,
            finishedAtUtc,
            hasOpenAnswers,
            isExpired,
            isInProgress,
            remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero);
    }

    private static async Task<QuizTeacherState> BuildTeacherStateAsync(
        ApplicationDbContext dbContext,
        int classId,
        QuizContent quiz,
        LiveQuestionState? currentQuestion,
        CancellationToken cancellationToken)
    {
        var studentStatuses = await BuildStudentStatusesAsync(
            dbContext,
            classId,
            currentQuestion,
            cancellationToken);

        var isComplete = currentQuestion is not null &&
            currentQuestion.QuestionIndex >= quiz.Questions.Count - 1 &&
            !currentQuestion.IsInProgress;

        return new QuizTeacherState(
            quiz.Title,
            currentQuestion is not null,
            isComplete,
            currentQuestion is null
                ? null
                : new QuizTeacherQuestionState(
                    currentQuestion.QuestionIndex,
                    quiz.Questions.Count,
                    currentQuestion.QuestionKey,
                    currentQuestion.Title,
                    currentQuestion.TimeoutSeconds,
                    currentQuestion.StartedAtUtc,
                    currentQuestion.FinishedAtUtc,
                    currentQuestion.IsInProgress,
                    currentQuestion.Remaining),
            studentStatuses);
    }

    private static async Task<IReadOnlyList<QuizStudentAnswerStatus>> BuildStudentStatusesAsync(
        ApplicationDbContext dbContext,
        int classId,
        LiveQuestionState? currentQuestion,
        CancellationToken cancellationToken)
    {
        var activeStudents = await dbContext.Students
            .AsNoTracking()
            .Where(student => student.ClassId == classId && student.IsActive)
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ThenBy(student => student.UserName)
            .Select(student => new
            {
                student.Id,
                student.UserName,
                student.DisplayName
            })
            .ToListAsync(cancellationToken);

        if (currentQuestion is null)
        {
            return activeStudents
                .Select(student => new QuizStudentAnswerStatus(student.Id, student.UserName, student.DisplayName, false, false))
                .ToList();
        }

        var answers = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer =>
                answer.QuestionIndex == currentQuestion.QuestionIndex &&
                answer.QuestionKey == currentQuestion.QuestionKey)
            .Select(answer => new
            {
                answer.StudentId,
                answer.Answer,
                answer.EndedAtUtc
            })
            .ToDictionaryAsync(answer => answer.StudentId, cancellationToken);

        return activeStudents
            .Select(student =>
            {
                answers.TryGetValue(student.Id, out var answer);

                return new QuizStudentAnswerStatus(
                    student.Id,
                    student.UserName,
                    student.DisplayName,
                    answer is not null && answer.Answer.Length > 0,
                    answer is not null && answer.EndedAtUtc is not null && answer.Answer.Length == 0);
            })
            .ToList();
    }

    private string? ValidateTeacherAccess(LoginState? loginState, ClassContextModel currentClass)
    {
        if (loginState is null)
        {
            return "Sign in as the teacher to manage quizzes.";
        }

        if (!loginState.IsTeacher)
        {
            return "Only teachers can manage quizzes.";
        }

        if (!string.Equals(loginState.ClassCode, currentClass.Code, StringComparison.OrdinalIgnoreCase))
        {
            return "Sign in as the teacher for this class to manage quizzes.";
        }

        var teacher = teacherOptions.Value;

        return string.Equals(loginState.UserName, teacher.UserName, StringComparison.OrdinalIgnoreCase)
            ? null
            : "Teacher login is required to manage quizzes.";
    }

    private sealed record LiveQuestionState(
        int QuestionIndex,
        string QuestionKey,
        string Title,
        int TimeoutSeconds,
        DateTime StartedAtUtc,
        DateTime? FinishedAtUtc,
        bool HasOpenAnswers,
        bool IsExpired,
        bool IsInProgress,
        TimeSpan Remaining);
}
