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

        var current = await GetCurrentQuestionAsync(
            dbContext,
            contentResult.Quiz,
            currentClass.ClassId,
            DateTime.UtcNow,
            cancellationToken);

        if (current is null)
        {
            return QuizAnswerPageStateResult.Success(new QuizAnswerPageState(false, false, false, "Waiting for the teacher to start a question."));
        }

        var answer = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(
                answer =>
                    answer.QuestionIndex == current.QuestionIndex &&
                    answer.QuestionKey == current.QuestionKey &&
                    answer.StudentId == studentResult.Student.Id)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (current.IsExpired)
        {
            await FinishExpiredQuestionAsync(dbContext, current, currentClass.ClassId, cancellationToken);

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
                new QuizAnswerPageState(false, true, false, $"Answer {answer.Answer} was submitted. Waiting for the next question."));
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
        string selectedAnswer,
        CancellationToken cancellationToken = default)
    {
        var selectedAnswerText = selectedAnswer.Trim();

        if (selectedAnswerText is not ("1" or "2" or "3" or "4"))
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

        var current = await GetCurrentQuestionAsync(
            dbContext,
            contentResult.Quiz,
            currentClass.ClassId,
            DateTime.UtcNow,
            cancellationToken);

        if (current is null)
        {
            return QuizActionResult.Failure("No question is available yet. Wait for the teacher to start.");
        }

        if (current.IsExpired)
        {
            await FinishExpiredQuestionAsync(dbContext, current, currentClass.ClassId, cancellationToken);

            return QuizActionResult.Failure("This question has finished.");
        }

        var answer = await dbContext.QuizAnswers
            .Where(answer =>
                answer.QuestionIndex == current.QuestionIndex &&
                answer.QuestionKey == current.QuestionKey &&
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

        answer.Answer = selectedAnswerText;
        answer.EndedAtUtc = DateTime.UtcNow;
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
        QuizContent quiz,
        int classId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var latestQuestion = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer => answer.Student != null && answer.Student.ClassId == classId)
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
                answer.Student != null &&
                answer.Student.ClassId == classId &&
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
        var isExpired = hasOpenAnswers && now >= startedAtUtc.AddSeconds(timeoutSeconds);

        return new CurrentQuestion(
            latestQuestion.QuestionIndex,
            latestQuestion.QuestionKey,
            questionContent?.Title ?? latestQuestion.QuestionText,
            timeoutSeconds,
            startedAtUtc,
            isExpired);
    }

    private static async Task FinishExpiredQuestionAsync(
        ApplicationDbContext dbContext,
        CurrentQuestion current,
        int classId,
        CancellationToken cancellationToken)
    {
        var finishedAtUtc = DateTime.UtcNow;

        var answers = await dbContext.QuizAnswers
            .Where(answer =>
                answer.Student != null &&
                answer.Student.ClassId == classId &&
                answer.QuestionIndex == current.QuestionIndex &&
                answer.QuestionKey == current.QuestionKey)
            .ToListAsync(cancellationToken);

        foreach (var answer in answers)
        {
            answer.EndedAtUtc ??= finishedAtUtc;
            answer.IsCorrect = answer.Answer.Length > 0 &&
                string.Equals(answer.Answer, answer.CorrectAnswer, StringComparison.Ordinal);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private sealed record StudentAccessResult(bool Succeeded, string Message, Student? Student)
    {
        public static StudentAccessResult Success(Student student) => new(true, string.Empty, student);

        public static StudentAccessResult Failure(string message) => new(false, message, null);
    }

    private sealed record CurrentQuestion(
        int QuestionIndex,
        string QuestionKey,
        string Title,
        int TimeoutSeconds,
        DateTime StartedAtUtc,
        bool IsExpired);
}
