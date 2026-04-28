using Microsoft.EntityFrameworkCore;
using MyClass.Core.Data;
using MyClass.Core.Data.Entities;
using MyClass.Core.Models;

namespace MyClass.Core.Services;

public sealed class QuizAnswerService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IQuizContentService quizContentService) : IQuizAnswerService
{
    public async Task<QuizAnswerPageStateResult> GetAnswerPageStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
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
            return QuizAnswerPageStateResult.Success(CreateState(false, false, false, contentResult.Message));
        }

        var current = await GetCurrentQuestionAsync(
            dbContext,
            contentResult.Quiz,
            currentClass.ClassId,
            DateTime.UtcNow,
            cancellationToken);

        if (current is null)
        {
            return QuizAnswerPageStateResult.Success(CreateState(false, false, false, "Waiting for the teacher to start a question."));
        }

        var answerChoices = CreateAnswerChoices(current.AnswerCount);

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
                    CreateState(false, true, false, "Answer submitted. Waiting for the next question.", current))
                : QuizAnswerPageStateResult.Success(
                    CreateState(false, false, true, "This question has finished.", current));
        }

        if (answer is null)
        {
            return QuizAnswerPageStateResult.Success(
                CreateState(false, false, false, "Waiting for the teacher to start a question."));
        }

        if (answer.Answer.Length > 0)
        {
            return QuizAnswerPageStateResult.Success(
                CreateState(false, true, false, $"Answer {answer.Answer} was submitted. Waiting for the next question.", current, answerChoices));
        }

        if (answer.EndedAtUtc is not null)
        {
            return QuizAnswerPageStateResult.Success(
                CreateState(false, false, true, "This question has finished.", current));
        }

        return QuizAnswerPageStateResult.Success(
            CreateState(true, false, false, "Choose an answer.", current, answerChoices));
    }

    public async Task<QuizActionResult> SubmitAnswerAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string selectedAnswer,
        CancellationToken cancellationToken = default)
    {
        var selectedAnswerText = selectedAnswer.Trim();

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

        if (!IsAnswerInRange(selectedAnswerText, current.AnswerCount))
        {
            return QuizActionResult.Failure($"Answer must be between 1 and {current.AnswerCount}.");
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
        ClassContext currentClass,
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
            questionContent?.AnswerCount ?? 4,
            timeoutSeconds,
            startedAtUtc,
            isExpired);
    }

    private static QuizAnswerPageState CreateState(
        bool hasInProgressAnswer,
        bool alreadyAnswered,
        bool failedNoAnswer,
        string message,
        CurrentQuestion? currentQuestion = null,
        IReadOnlyList<string>? answerChoices = null)
    {
        return new QuizAnswerPageState(
            hasInProgressAnswer,
            alreadyAnswered,
            failedNoAnswer,
            message,
            currentQuestion?.QuestionKey,
            currentQuestion?.Title,
            answerChoices ?? []);
    }

    private static IReadOnlyList<string> CreateAnswerChoices(int answerCount)
    {
        return Enumerable.Range(1, answerCount)
            .Select(answer => answer.ToString())
            .ToList();
    }

    private static bool IsAnswerInRange(string answer, int answerCount)
    {
        return int.TryParse(answer, out var answerNumber) &&
            answerNumber >= 1 &&
            answerNumber <= answerCount &&
            string.Equals(answer, answerNumber.ToString(), StringComparison.Ordinal);
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
        int AnswerCount,
        int TimeoutSeconds,
        DateTime StartedAtUtc,
        bool IsExpired);
}


