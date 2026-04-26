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

        var session = await GetActiveSessionAsync(dbContext, currentClass.ClassId, cancellationToken);
        var currentQuestion = session is null
            ? null
            : await GetCurrentQuestionAsync(dbContext, session.Id, session.ActiveQuestionIndex, cancellationToken);

        if (session is not null &&
            currentQuestion is not null &&
            currentQuestion.Status == QuizQuestionStatus.InProgress &&
            HasQuestionExpired(currentQuestion, now))
        {
            await FinishQuestionAsync(dbContext, session, currentQuestion, contentResult.Quiz.Questions.Count, now, cancellationToken);
        }

        var state = await BuildTeacherStateAsync(
            dbContext,
            currentClass.ClassId,
            contentResult.Quiz,
            session,
            currentQuestion,
            now,
            cancellationToken);

        return QuizTeacherStateResult.Success(state);
    }

    public async Task<QuizActionResult> StartQuestionAsync(
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

        var session = await GetActiveSessionAsync(dbContext, currentClass.ClassId, cancellationToken);

        if (session is null)
        {
            session = new QuizSession
            {
                ClassId = currentClass.ClassId,
                Title = contentResult.Quiz.Title,
                Status = QuizSessionStatus.InProgress,
                ActiveQuestionIndex = 0,
                StartedAtUtc = DateTime.UtcNow
            };

            var firstQuestion = await CreateQuestionAsync(
                dbContext,
                currentClass.ClassId,
                session,
                contentResult.Quiz.Questions[0],
                cancellationToken);

            session.Questions.Add(firstQuestion);
            dbContext.QuizSessions.Add(session);
            await dbContext.SaveChangesAsync(cancellationToken);

            return QuizActionResult.Success("Quiz started.");
        }

        var currentQuestion = await GetCurrentQuestionAsync(
            dbContext,
            session.Id,
            session.ActiveQuestionIndex,
            cancellationToken);

        if (currentQuestion is not null)
        {
            return currentQuestion.Status == QuizQuestionStatus.InProgress
                ? QuizActionResult.Success("Question is already in progress.")
                : QuizActionResult.Failure("Use Next to advance to the next question.");
        }

        var questionContent = contentResult.Quiz.Questions[session.ActiveQuestionIndex];
        dbContext.QuizSessionQuestions.Add(await CreateQuestionAsync(
            dbContext,
            currentClass.ClassId,
            session,
            questionContent,
            cancellationToken));

        await dbContext.SaveChangesAsync(cancellationToken);

        return QuizActionResult.Success("Question started.");
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

        var session = await GetActiveSessionAsync(dbContext, currentClass.ClassId, cancellationToken);

        if (session is null)
        {
            return QuizActionResult.Failure("No quiz session is in progress.");
        }

        var question = await GetCurrentQuestionAsync(
            dbContext,
            session.Id,
            session.ActiveQuestionIndex,
            cancellationToken);

        if (question is null)
        {
            return QuizActionResult.Failure("No question is in progress.");
        }

        if (question.Status == QuizQuestionStatus.Finished)
        {
            return QuizActionResult.Success("Question is already finished.");
        }

        await FinishQuestionAsync(
            dbContext,
            session,
            question,
            contentResult.Quiz.Questions.Count,
            DateTime.UtcNow,
            cancellationToken);

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

        var session = await GetActiveSessionAsync(dbContext, currentClass.ClassId, cancellationToken);

        if (session is null)
        {
            return QuizActionResult.Failure("No quiz session is in progress.");
        }

        var currentQuestion = await GetCurrentQuestionAsync(
            dbContext,
            session.Id,
            session.ActiveQuestionIndex,
            cancellationToken);

        if (currentQuestion is not null && currentQuestion.Status == QuizQuestionStatus.InProgress)
        {
            return QuizActionResult.Failure("Finish the current question before moving next.");
        }

        var nextIndex = session.ActiveQuestionIndex + 1;

        if (nextIndex >= contentResult.Quiz.Questions.Count)
        {
            session.Status = QuizSessionStatus.Completed;
            session.CompletedAtUtc ??= DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);

            return QuizActionResult.Success("Quiz complete.");
        }

        session.ActiveQuestionIndex = nextIndex;

        dbContext.QuizSessionQuestions.Add(await CreateQuestionAsync(
            dbContext,
            currentClass.ClassId,
            session,
            contentResult.Quiz.Questions[nextIndex],
            cancellationToken));

        await dbContext.SaveChangesAsync(cancellationToken);

        return QuizActionResult.Success("Next question started.");
    }

    private async Task<QuizSessionQuestion> CreateQuestionAsync(
        ApplicationDbContext dbContext,
        int classId,
        QuizSession session,
        QuizQuestionContent questionContent,
        CancellationToken cancellationToken)
    {
        var activeStudents = await dbContext.Students
            .Where(student => student.ClassId == classId && student.IsActive)
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ThenBy(student => student.UserName)
            .ToListAsync(cancellationToken);

        return new QuizSessionQuestion
        {
            QuizSession = session,
            QuizSessionId = session.Id,
            QuestionIndex = questionContent.Index,
            QuestionKey = questionContent.Key,
            Title = questionContent.Title,
            TimeoutSeconds = questionContent.TimeoutSeconds,
            CorrectAnswer = questionContent.CorrectAnswer,
            Status = QuizQuestionStatus.InProgress,
            StartedAtUtc = DateTime.UtcNow,
            Answers = activeStudents
                .Select(student => new QuizAnswer
                {
                    StudentId = student.Id,
                    Status = QuizAnswerStatus.InProgress,
                    CreatedAtUtc = DateTime.UtcNow
                })
                .ToList()
        };
    }

    private static async Task<QuizSession?> GetActiveSessionAsync(
        ApplicationDbContext dbContext,
        int classId,
        CancellationToken cancellationToken)
    {
        return await dbContext.QuizSessions
            .Where(session => session.ClassId == classId && session.Status == QuizSessionStatus.InProgress)
            .OrderByDescending(session => session.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static async Task<QuizSessionQuestion?> GetCurrentQuestionAsync(
        ApplicationDbContext dbContext,
        int sessionId,
        int activeQuestionIndex,
        CancellationToken cancellationToken)
    {
        return await dbContext.QuizSessionQuestions
            .SingleOrDefaultAsync(
                question =>
                    question.QuizSessionId == sessionId &&
                    question.QuestionIndex == activeQuestionIndex,
                cancellationToken);
    }

    private static async Task FinishQuestionAsync(
        ApplicationDbContext dbContext,
        QuizSession session,
        QuizSessionQuestion question,
        int questionCount,
        DateTime finishedAtUtc,
        CancellationToken cancellationToken)
    {
        question.Status = QuizQuestionStatus.Finished;
        question.FinishedAtUtc ??= finishedAtUtc;

        await dbContext.QuizAnswers
            .Where(answer =>
                answer.QuizSessionQuestionId == question.Id &&
                answer.Status == QuizAnswerStatus.InProgress)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(answer => answer.Status, QuizAnswerStatus.FailedNoAnswer)
                    .SetProperty(answer => answer.IsCorrect, false),
                cancellationToken);

        if (question.QuestionIndex >= questionCount - 1)
        {
            session.Status = QuizSessionStatus.Completed;
            session.CompletedAtUtc ??= finishedAtUtc;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<QuizTeacherState> BuildTeacherStateAsync(
        ApplicationDbContext dbContext,
        int classId,
        QuizContent quiz,
        QuizSession? session,
        QuizSessionQuestion? currentQuestion,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var studentStatuses = await BuildStudentStatusesAsync(
            dbContext,
            classId,
            currentQuestion?.Id,
            cancellationToken);

        return new QuizTeacherState(
            quiz.Title,
            session is not null,
            session?.Status == QuizSessionStatus.Completed,
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
                    currentQuestion.Status == QuizQuestionStatus.InProgress,
                    GetRemaining(currentQuestion, now)),
            studentStatuses);
    }

    private static async Task<IReadOnlyList<QuizStudentAnswerStatus>> BuildStudentStatusesAsync(
        ApplicationDbContext dbContext,
        int classId,
        int? currentQuestionId,
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

        if (currentQuestionId is null)
        {
            return activeStudents
                .Select(student => new QuizStudentAnswerStatus(student.Id, student.UserName, student.DisplayName, false, false))
                .ToList();
        }

        var answers = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer => answer.QuizSessionQuestionId == currentQuestionId)
            .Select(answer => new
            {
                answer.StudentId,
                answer.Status
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
                    answer?.Status == QuizAnswerStatus.Answered,
                    answer?.Status == QuizAnswerStatus.FailedNoAnswer);
            })
            .ToList();
    }

    private static bool HasQuestionExpired(QuizSessionQuestion question, DateTime now)
    {
        return now >= question.StartedAtUtc.AddSeconds(question.TimeoutSeconds);
    }

    private static TimeSpan GetRemaining(QuizSessionQuestion question, DateTime now)
    {
        if (question.Status == QuizQuestionStatus.Finished)
        {
            return TimeSpan.Zero;
        }

        var remaining = question.StartedAtUtc.AddSeconds(question.TimeoutSeconds) - now;

        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
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
}
