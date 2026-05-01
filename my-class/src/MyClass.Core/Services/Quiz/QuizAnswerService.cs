using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Core.Data;
using MyClass.Core.Data.Entities;
using MyClass.Core.Models;
using MyClass.Core.Options;

namespace MyClass.Core.Services;

public sealed class QuizAnswerService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IQuizContentService quizContentService,
    IActiveQuizSelectionService activeQuizSelectionService,
    IOptions<QuizOptions> quizOptions) : IQuizAnswerService
{
    public async Task<Result<QuizAnswerPageState>> GetAnswerPageStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var studentResult = await ValidateStudentAccessAsync(dbContext, loginState, currentClass, cancellationToken);

        if (!studentResult.Succeeded || studentResult.Value is null)
        {
            return Result<QuizAnswerPageState>.Failure(studentResult.Message);
        }

        var activeQuizPath = activeQuizSelectionService.GetSelectedQuizPath(currentClass.ClassId);

        var contentResult = await quizContentService.LoadQuizAsync(activeQuizPath, cancellationToken);

        if (!contentResult.Succeeded || contentResult.Value is null)
        {
            return Result<QuizAnswerPageState>.Success(CreateState(false, false, false, contentResult.Message, activeQuizPath: activeQuizPath));
        }

        var current = await GetCurrentQuestionAsync(
            dbContext,
            contentResult.Value,
            currentClass.ClassId,
            DateTime.UtcNow,
            cancellationToken);

        if (current is null)
        {
            return Result<QuizAnswerPageState>.Success(CreateState(
                false,
                false,
                false,
                "Waiting for the teacher to start a question.",
                contentResult.Value.Title,
                progressItems: CreateNeutralProgress(contentResult.Value),
                activeQuizPath: activeQuizPath));
        }

        var answerChoices = CreateAnswerChoices(current.AnswerCount);
        var progressItems = await BuildQuestionProgressAsync(
            dbContext,
            contentResult.Value,
            current,
            studentResult.Value.Id,
            cancellationToken);

        var answer = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(
                    answer =>
                    answer.QuestionIndex == current.QuestionIndex &&
                    answer.QuestionKey == current.QuestionKey &&
                    answer.StudentId == studentResult.Value.Id)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (current.IsExpired)
        {
            await FinishExpiredQuestionAsync(dbContext, current, currentClass.ClassId, cancellationToken);

            return answer is not null && answer.Answer.Length > 0
                ? Result<QuizAnswerPageState>.Success(
                    CreateState(false, true, false, "Answer submitted. Waiting for the next question.", contentResult.Value.Title, current, progressItems: progressItems, activeQuizPath: activeQuizPath))
                : Result<QuizAnswerPageState>.Success(
                    CreateState(false, false, true, "This question has finished.", contentResult.Value.Title, current, progressItems: progressItems, activeQuizPath: activeQuizPath));
        }

        if (current.IsAnswerRevealed)
        {
            var hasAnswered = answer is not null && answer.Answer.Length > 0;
            var isCorrect = hasAnswered ? answer!.IsCorrect : null as bool?;

            return Result<QuizAnswerPageState>.Success(
                CreateState(
                    false,
                    hasAnswered,
                    !hasAnswered,
                    GetRevealMessage(isCorrect),
                    contentResult.Value.Title,
                    current,
                    answerChoices,
                    progressItems,
                    isCorrect,
                    hasAnswered ? answer!.EndedAtUtc : null,
                    hasAnswered && answer!.EndedAtUtc is not null
                        ? answer.EndedAtUtc.Value - answer.StartedAtUtc
                        : null,
                    activeQuizPath));
        }

        if (answer is null)
        {
            return Result<QuizAnswerPageState>.Success(
                CreateState(false, false, false, "Waiting for the teacher to start a question.", contentResult.Value.Title, progressItems: progressItems, activeQuizPath: activeQuizPath));
        }

        if (answer.Answer.Length > 0)
        {
            return Result<QuizAnswerPageState>.Success(
                CreateState(false, true, false, $"Answer {answer.Answer} was submitted. Waiting for the next question.", contentResult.Value.Title, current, answerChoices, progressItems, activeQuizPath: activeQuizPath));
        }

        if (answer.EndedAtUtc is not null)
        {
            return Result<QuizAnswerPageState>.Success(
                CreateState(false, false, true, "This question has finished.", contentResult.Value.Title, current, progressItems: progressItems, activeQuizPath: activeQuizPath));
        }

        return Result<QuizAnswerPageState>.Success(
            CreateState(true, false, false, "Choose an answer.", contentResult.Value.Title, current, answerChoices, progressItems, activeQuizPath: activeQuizPath));
    }

    public async Task<Result<bool>> SubmitAnswerAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string selectedAnswer,
        CancellationToken cancellationToken = default)
    {
        var selectedAnswerText = selectedAnswer.Trim();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var studentResult = await ValidateStudentAccessAsync(dbContext, loginState, currentClass, cancellationToken);

        if (!studentResult.Succeeded || studentResult.Value is null)
        {
            return Result<bool>.Failure(studentResult.Message);
        }

        var activeQuizPath = activeQuizSelectionService.GetSelectedQuizPath(currentClass.ClassId);

        var contentResult = await quizContentService.LoadQuizAsync(activeQuizPath, cancellationToken);

        if (!contentResult.Succeeded || contentResult.Value is null)
        {
            return Result<bool>.Failure(contentResult.Message);
        }

        var current = await GetCurrentQuestionAsync(
            dbContext,
            contentResult.Value,
            currentClass.ClassId,
            DateTime.UtcNow,
            cancellationToken);

        if (current is null)
        {
            return Result<bool>.Failure("No question is available yet. Wait for the teacher to start.");
        }

        if (!IsAnswerInRange(selectedAnswerText, current.AnswerCount))
        {
            return Result<bool>.Failure($"Answer must be between 1 and {current.AnswerCount}.");
        }

        if (current.IsExpired)
        {
            await FinishExpiredQuestionAsync(dbContext, current, currentClass.ClassId, cancellationToken);

            return Result<bool>.Failure("This question has finished.");
        }

        var answer = await dbContext.QuizAnswers
            .Where(answer =>
                answer.QuestionIndex == current.QuestionIndex &&
                answer.QuestionKey == current.QuestionKey &&
                answer.StudentId == studentResult.Value.Id)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (answer is null)
        {
            return Result<bool>.Failure("No answer record is available yet. Wait for the teacher to start.");
        }

        if (answer.Answer.Length > 0)
        {
            return Result<bool>.Failure("You already answered this question.");
        }

        if (answer.EndedAtUtc is not null)
        {
            return Result<bool>.Failure("This question has finished.");
        }

        answer.Answer = selectedAnswerText;
        answer.EndedAtUtc = DateTime.UtcNow;
        answer.IsCorrect = string.Equals(selectedAnswerText, answer.CorrectAnswer, StringComparison.Ordinal);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Answer submitted.");
    }

    private static async Task<Result<Student>> ValidateStudentAccessAsync(
        ApplicationDbContext dbContext,
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken)
    {
        if (loginState is null)
        {
            return Result<Student>.Failure("Sign in as a student to answer quizzes.");
        }

        if (loginState.IsTeacher)
        {
            return Result<Student>.Failure("Teachers cannot submit student quiz answers.");
        }

        if (!string.Equals(loginState.ClassCode, currentClass.Code, StringComparison.OrdinalIgnoreCase))
        {
            return Result<Student>.Failure("Sign in as a student for this class to answer quizzes.");
        }

        var normalizedUserName = loginState.UserName.Trim().ToLower();

        var student = await dbContext.Students
            .SingleOrDefaultAsync(
                student =>
                    student.ClassId == currentClass.ClassId &&
                    student.UserName.ToLower() == normalizedUserName,
                cancellationToken);

        return student is null
            ? Result<Student>.Failure("Student login is required to answer quizzes.")
            : Result<Student>.Success(student);
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
            .Where(answer =>
                answer.Student != null &&
                answer.Student.ClassId == classId)
            .OrderByDescending(answer => answer.StartedAtUtc)
            .ThenByDescending(answer => answer.QuestionIndex)
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
                answer.EndedAtUtc,
                answer.AnswerRevealedAtUtc
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
        var answerRevealedAtUtc = rows
            .Where(row => row.AnswerRevealedAtUtc is not null)
            .Select(row => row.AnswerRevealedAtUtc)
            .Max();
        var remaining = isExpired || !hasOpenAnswers
            ? TimeSpan.Zero
            : startedAtUtc.AddSeconds(timeoutSeconds) - now;

        return new CurrentQuestion(
            latestQuestion.QuestionIndex,
            latestQuestion.QuestionKey,
            questionContent?.Title ?? latestQuestion.QuestionText,
            questionContent?.AnswerCount ?? 4,
            quiz.Questions.Count,
            timeoutSeconds,
            startedAtUtc,
            answerRevealedAtUtc,
            hasOpenAnswers && !isExpired,
            remaining,
            isExpired);
    }

    private static QuizAnswerPageState CreateState(
        bool hasInProgressAnswer,
        bool alreadyAnswered,
        bool failedNoAnswer,
        string message,
        string quizTitle = "Quiz Answer",
        CurrentQuestion? currentQuestion = null,
        IReadOnlyList<string>? answerChoices = null,
        IReadOnlyList<QuizQuestionProgressItem>? progressItems = null,
        bool? isCorrect = null,
        DateTime? answeredAtUtc = null,
        TimeSpan? answerElapsed = null,
        string? activeQuizPath = null)
    {
        return new QuizAnswerPageState(
            hasInProgressAnswer,
            alreadyAnswered,
            failedNoAnswer,
            message,
            quizTitle,
            activeQuizPath,
            currentQuestion?.QuestionKey,
            currentQuestion?.Title,
            currentQuestion?.QuestionIndex,
            currentQuestion?.QuestionCount,
            currentQuestion?.IsAnswerRevealed == true,
            isCorrect,
            answeredAtUtc,
            answerElapsed,
            currentQuestion?.IsAnswerRevealed == true ? message : null,
            currentQuestion?.IsInProgress == true,
            currentQuestion?.Remaining ?? TimeSpan.Zero,
            progressItems ?? [],
            answerChoices ?? []);
    }

    private static IReadOnlyList<QuizQuestionProgressItem> CreateNeutralProgress(QuizContent quiz)
    {
        return quiz.Questions
            .Select(question => new QuizQuestionProgressItem(
                question.Index,
                false,
                false,
                QuizQuestionProgressResult.Neutral))
            .ToList();
    }

    private static async Task<IReadOnlyList<QuizQuestionProgressItem>> BuildQuestionProgressAsync(
        ApplicationDbContext dbContext,
        QuizContent quiz,
        CurrentQuestion currentQuestion,
        int studentId,
        CancellationToken cancellationToken)
    {
        var answerRows = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer => answer.StudentId == studentId)
            .Select(answer => new
            {
                answer.QuestionIndex,
                answer.Answer,
                answer.EndedAtUtc,
                answer.AnswerRevealedAtUtc,
                answer.IsCorrect,
                answer.StartedAtUtc
            })
            .ToListAsync(cancellationToken);

        var answers = answerRows
            .GroupBy(answer => answer.QuestionIndex)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderByDescending(answer => answer.StartedAtUtc)
                    .First());

        return quiz.Questions
            .Select(question =>
            {
                var isCurrent = question.Index == currentQuestion.QuestionIndex;
                var isPast = question.Index < currentQuestion.QuestionIndex;
                answers.TryGetValue(question.Index, out var answer);

                var result = GetQuestionProgressResult(answer?.Answer, answer?.EndedAtUtc, answer?.AnswerRevealedAtUtc, answer?.IsCorrect, isPast, isCurrent);

                return new QuizQuestionProgressItem(
                    question.Index,
                    isCurrent,
                    isPast || isCurrent,
                    result);
            })
            .ToList();
    }

    private static QuizQuestionProgressResult GetQuestionProgressResult(
        string? answer,
        DateTime? endedAtUtc,
        DateTime? answerRevealedAtUtc,
        bool? isCorrect,
        bool isPast,
        bool isCurrent)
    {
        if (!isPast && !(isCurrent && answerRevealedAtUtc is not null))
        {
            return QuizQuestionProgressResult.Neutral;
        }

        if (endedAtUtc is null)
        {
            return QuizQuestionProgressResult.Neutral;
        }

        if (string.IsNullOrEmpty(answer))
        {
            return QuizQuestionProgressResult.Missed;
        }

        return isCorrect == true
            ? QuizQuestionProgressResult.Correct
            : QuizQuestionProgressResult.Incorrect;
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

    private string GetRevealMessage(bool? isCorrect)
    {
        var messages = quizOptions.Value.RevealMessages;

        return isCorrect switch
        {
            true => GetConfiguredMessage(messages.Correct, "Correct. Good job!"),
            false => GetConfiguredMessage(messages.Incorrect, "Incorrect. Not this time."),
            _ => GetConfiguredMessage(messages.NoAnswer, "No answer submitted. Stay focused!")
        };
    }

    private static string GetConfiguredMessage(string? configuredMessage, string fallback)
    {
        return string.IsNullOrWhiteSpace(configuredMessage)
            ? fallback
            : configuredMessage.Trim();
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

    private sealed record CurrentQuestion(
        int QuestionIndex,
        string QuestionKey,
        string Title,
        int AnswerCount,
        int QuestionCount,
        int TimeoutSeconds,
        DateTime StartedAtUtc,
        DateTime? AnswerRevealedAtUtc,
        bool IsInProgress,
        TimeSpan Remaining,
        bool IsExpired)
    {
        public bool IsAnswerRevealed => AnswerRevealedAtUtc is not null;
    }
}


