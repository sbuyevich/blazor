using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MyClass.Data;
using MyClass.Data.Entities;
using MyClass.Services.Auth;
using ClassContextModel = MyClass.Services.ClassContext.ClassContext;

namespace MyClass.Services.Quiz;

public sealed class QuizAnswerService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IQuizContentService quizContentService) : IQuizAnswerService
{
    public async Task<QuizAnswerPageStateResult> GetAnswerPageStateAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var studentResult = await ValidateStudentAccessAsync(dbContext, loginState, currentClass, cancellationToken);

        if (!studentResult.Succeeded || studentResult.Student is null)
        {
            return QuizAnswerPageStateResult.Failure(studentResult.Message);
        }

        var contentResult = await quizContentService.LoadQuizAsync(cancellationToken);

        if (!contentResult.Succeeded || contentResult.Quiz is null)
        {
            return QuizAnswerPageStateResult.Success(new QuizAnswerPageState(false, false, false, contentResult.Message));
        }

        var current = await GetCurrentQuestionAsync(dbContext, currentClass.ClassId, cancellationToken);

        if (current is null)
        {
            return QuizAnswerPageStateResult.Success(new QuizAnswerPageState(false, false, false, "Waiting for the teacher to start a question."));
        }

        var answer = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(
                answer =>
                    answer.QuestionIndex == current.Question.QuestionIndex &&
                    answer.QuestionKey == current.Question.QuestionKey &&
                    answer.StudentId == studentResult.Student.Id)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (HasQuestionExpired(current.Question, DateTime.UtcNow))
        {
            await FinishExpiredQuestionAsync(dbContext, current, contentResult.Quiz.Questions.Count, cancellationToken);

            return answer is not null && answer.Answer.Length > 0
                ? QuizAnswerPageStateResult.Success(
                    new QuizAnswerPageState(false, true, false, "Answer submitted. Waiting for the next question."))
                : QuizAnswerPageStateResult.Success(
                    new QuizAnswerPageState(false, false, true, "This question has finished."));
        }

        if (answer is null)
        {
            return QuizAnswerPageStateResult.Success(
                new QuizAnswerPageState(false, false, false, "Waiting for the teacher to start a question."));
        }

        if (answer.Answer.Length > 0)
        {
            return QuizAnswerPageStateResult.Success(
                new QuizAnswerPageState(false, true, false, "Answer submitted. Waiting for the next question."));
        }

        if (answer.EndedAtUtc is not null)
        {
            return QuizAnswerPageStateResult.Success(
                new QuizAnswerPageState(false, false, true, "This question has finished."));
        }

        return QuizAnswerPageStateResult.Success(
            new QuizAnswerPageState(true, false, false, "Choose an answer."));
    }

    public async Task<QuizActionResult> SubmitAnswerAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        int selectedAnswer,
        CancellationToken cancellationToken = default)
    {
        if (selectedAnswer is < 1 or > 4)
        {
            return QuizActionResult.Failure("Answer must be 1, 2, 3, or 4.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var studentResult = await ValidateStudentAccessAsync(dbContext, loginState, currentClass, cancellationToken);

        if (!studentResult.Succeeded || studentResult.Student is null)
        {
            return QuizActionResult.Failure(studentResult.Message);
        }

        var contentResult = await quizContentService.LoadQuizAsync(cancellationToken);

        if (!contentResult.Succeeded || contentResult.Quiz is null)
        {
            return QuizActionResult.Failure(contentResult.Message);
        }

        var current = await GetCurrentQuestionAsync(dbContext, currentClass.ClassId, cancellationToken);

        if (current is null)
        {
            return QuizActionResult.Failure("No question is available yet. Wait for the teacher to start.");
        }

        if (HasQuestionExpired(current.Question, DateTime.UtcNow))
        {
            await FinishExpiredQuestionAsync(dbContext, current, contentResult.Quiz.Questions.Count, cancellationToken);

            return QuizActionResult.Failure("This question has finished.");
        }

        var answer = await dbContext.QuizAnswers
            .Where(answer =>
                answer.QuestionIndex == current.Question.QuestionIndex &&
                answer.QuestionKey == current.Question.QuestionKey &&
                answer.StudentId == studentResult.Student.Id)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (answer is null)
        {
            return QuizActionResult.Failure("No answer record is available yet. Wait for the teacher to start.");
        }

        if (answer.Answer.Length > 0)
        {
            return QuizActionResult.Failure("You already answered this question.");
        }

        if (answer.EndedAtUtc is not null)
        {
            return QuizActionResult.Failure("This question has finished.");
        }

        var submittedAtUtc = DateTime.UtcNow;
        var selectedAnswerText = selectedAnswer.ToString(CultureInfo.InvariantCulture);

        answer.Answer = selectedAnswerText;
        answer.EndedAtUtc = submittedAtUtc;
        answer.IsCorrect = string.Equals(selectedAnswerText, answer.CorrectAnswer, StringComparison.Ordinal);

        await dbContext.SaveChangesAsync(cancellationToken);

        return QuizActionResult.Success("Answer submitted.");
    }

    private static async Task<StudentAccessResult> ValidateStudentAccessAsync(
        ApplicationDbContext dbContext,
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken)
    {
        if (loginState is null)
        {
            return StudentAccessResult.Failure("Sign in as a student to answer quizzes.");
        }

        if (loginState.IsTeacher)
        {
            return StudentAccessResult.Failure("Teachers cannot submit student quiz answers.");
        }

        if (!string.Equals(loginState.ClassCode, currentClass.Code, StringComparison.OrdinalIgnoreCase))
        {
            return StudentAccessResult.Failure("Sign in as a student for this class to answer quizzes.");
        }

        var normalizedUserName = loginState.UserName.Trim().ToLower();

        var student = await dbContext.Students
            .SingleOrDefaultAsync(
                student =>
                    student.ClassId == currentClass.ClassId &&
                    student.UserName.ToLower() == normalizedUserName,
                cancellationToken);

        return student is null
            ? StudentAccessResult.Failure("Student login is required to answer quizzes.")
            : StudentAccessResult.Success(student);
    }

    private static async Task<CurrentQuestion?> GetCurrentQuestionAsync(
        ApplicationDbContext dbContext,
        int classId,
        CancellationToken cancellationToken)
    {
        var session = await dbContext.QuizSessions
            .Where(session => session.ClassId == classId && session.Status == QuizSessionStatus.InProgress)
            .OrderByDescending(session => session.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return null;
        }

        var question = await dbContext.QuizSessionQuestions
            .SingleOrDefaultAsync(
                question =>
                    question.QuizSessionId == session.Id &&
                    question.QuestionIndex == session.ActiveQuestionIndex &&
                    question.Status == QuizQuestionStatus.InProgress,
                cancellationToken);

        return question is null ? null : new CurrentQuestion(session, question);
    }

    private static async Task FinishExpiredQuestionAsync(
        ApplicationDbContext dbContext,
        CurrentQuestion current,
        int questionCount,
        CancellationToken cancellationToken)
    {
        var finishedAtUtc = DateTime.UtcNow;

        current.Question.Status = QuizQuestionStatus.Finished;
        current.Question.FinishedAtUtc ??= finishedAtUtc;

        var answers = await dbContext.QuizAnswers
            .Where(answer =>
                answer.QuestionIndex == current.Question.QuestionIndex &&
                answer.QuestionKey == current.Question.QuestionKey &&
                answer.EndedAtUtc == null)
            .ToListAsync(cancellationToken);

        foreach (var answer in answers)
        {
            answer.EndedAtUtc = finishedAtUtc;
            answer.IsCorrect = answer.Answer.Length > 0 &&
                string.Equals(answer.Answer, answer.CorrectAnswer, StringComparison.Ordinal);
        }

        if (current.Question.QuestionIndex >= questionCount - 1)
        {
            current.Session.Status = QuizSessionStatus.Completed;
            current.Session.CompletedAtUtc ??= finishedAtUtc;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static bool HasQuestionExpired(QuizSessionQuestion question, DateTime now)
    {
        return now >= question.StartedAtUtc.AddSeconds(question.TimeoutSeconds);
    }

    private sealed record StudentAccessResult(bool Succeeded, string Message, Student? Student)
    {
        public static StudentAccessResult Success(Student student) => new(true, string.Empty, student);

        public static StudentAccessResult Failure(string message) => new(false, message, null);
    }

    private sealed record CurrentQuestion(QuizSession Session, QuizSessionQuestion Question);
}
