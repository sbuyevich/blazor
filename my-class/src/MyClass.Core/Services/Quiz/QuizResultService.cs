using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Core.Data;
using MyClass.Core.Models;
using MyClass.Core.Options;

namespace MyClass.Core.Services;

public sealed class QuizResultService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IQuizContentService quizContentService,
    IActiveQuizSelectionService activeQuizSelectionService,
    IOptions<TeacherOptions> teacherOptions) : IQuizResultService
{
    public async Task<Result<QuizResultPageState>> GetResultPageStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState, currentClass);

        if (authorizationMessage is not null)
        {
            return Result<QuizResultPageState>.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var answerRows = await dbContext.QuizAnswers
            .AsNoTracking()
            .Where(answer =>
                answer.Student != null &&
                answer.Student.ClassId == currentClass.ClassId)
            .OrderBy(answer => answer.QuestionIndex)
            .ThenBy(answer => answer.QuestionKey)
            .ThenBy(answer => answer.StudentDisplayName)
            .Select(answer => new QuizResultAnswerRow(
                answer.StudentId,
                answer.StudentDisplayName,
                answer.QuestionIndex,
                answer.QuestionKey,
                answer.QuestionText,
                answer.CorrectAnswer,
                answer.Answer,
                answer.StartedAtUtc,
                answer.EndedAtUtc,
                answer.IsCorrect))
            .ToListAsync(cancellationToken);

        if (answerRows.Count == 0)
        {
            return Result<QuizResultPageState>.Success(CreateEmptyState());
        }

        var activeQuizPath = activeQuizSelectionService.GetSelectedQuizPath(currentClass.ClassId);
        var contentResult = await quizContentService.LoadQuizAsync(activeQuizPath, cancellationToken);

        if (!contentResult.Succeeded || contentResult.Value is null)
        {
            return Result<QuizResultPageState>.Failure(contentResult.Message);
        }

        var questionTimeLimits = contentResult.Value.Questions.ToDictionary(
            question => CreateQuestionKey(question.Index, question.Key),
            question => TimeSpan.FromSeconds(question.TimeoutSeconds));
        var defaultQuestionTime = TimeSpan.FromSeconds(contentResult.Value.TimeLimitSeconds);

        var rowsWithTime = answerRows
            .Select(row => new QuizResultCalculatedRow(row, GetAnswerTime(row, questionTimeLimits, defaultQuestionTime)))
            .ToList();

        return Result<QuizResultPageState>.Success(
            new QuizResultPageState(
                CreateStudentSummaries(rowsWithTime),
                CreateQuestionSummaries(rowsWithTime),
                CreateExportRows(rowsWithTime)));
    }

    private static QuizResultPageState CreateEmptyState()
    {
        return new QuizResultPageState([], [], []);
    }

    private static IReadOnlyList<QuizResultStudentSummary> CreateStudentSummaries(
        IReadOnlyList<QuizResultCalculatedRow> rows)
    {
        return rows
            .GroupBy(row => new
            {
                row.Answer.StudentId,
                row.Answer.StudentDisplayName
            })
            .OrderBy(group => group.Key.StudentDisplayName)
            .Select(group =>
            {
                var correctCount = group.Count(row => row.Answer.IsCorrect);
                var incorrectCount = group.Count() - correctCount;

                return new QuizResultStudentSummary(
                    group.Key.StudentId,
                    group.Key.StudentDisplayName,
                    correctCount,
                    incorrectCount,
                    CalculatePercentCorrect(correctCount, group.Count()),
                    SumAnswerTime(group));
            })
            .ToList();
    }

    private static IReadOnlyList<QuizResultQuestionSummary> CreateQuestionSummaries(
        IReadOnlyList<QuizResultCalculatedRow> rows)
    {
        return rows
            .GroupBy(row => new
            {
                row.Answer.QuestionIndex,
                row.Answer.QuestionKey,
                row.Answer.QuestionText
            })
            .OrderBy(group => group.Key.QuestionIndex)
            .ThenBy(group => group.Key.QuestionKey)
            .Select(group =>
            {
                var correctCount = group.Count(row => row.Answer.IsCorrect);
                var incorrectCount = group.Count() - correctCount;

                return new QuizResultQuestionSummary(
                    group.Key.QuestionIndex,
                    group.Key.QuestionKey,
                    group.Key.QuestionText,
                    correctCount,
                    incorrectCount,
                    CalculatePercentCorrect(correctCount, group.Count()),
                    SumAnswerTime(group));
            })
            .ToList();
    }

    private static IReadOnlyList<QuizResultExportRow> CreateExportRows(
        IReadOnlyList<QuizResultCalculatedRow> rows)
    {
        return rows
            .Select(row => new QuizResultExportRow(
                row.Answer.StudentDisplayName,
                row.Answer.QuestionText,
                row.Answer.CorrectAnswer,
                row.Answer.Answer,
                row.AnswerTime,
                row.Answer.IsCorrect))
            .ToList();
    }

    private static TimeSpan GetAnswerTime(
        QuizResultAnswerRow row,
        IReadOnlyDictionary<string, TimeSpan> questionTimeLimits,
        TimeSpan defaultQuestionTime)
    {
        if (string.IsNullOrWhiteSpace(row.Answer))
        {
            return questionTimeLimits.TryGetValue(CreateQuestionKey(row.QuestionIndex, row.QuestionKey), out var timeout)
                ? timeout
                : defaultQuestionTime;
        }

        return row.EndedAtUtc is null
            ? TimeSpan.Zero
            : row.EndedAtUtc.Value - row.StartedAtUtc;
    }

    private static double CalculatePercentCorrect(int correctCount, int totalCount)
    {
        return totalCount == 0
            ? 0
            : (double)correctCount / totalCount * 100;
    }

    private static TimeSpan SumAnswerTime(IEnumerable<QuizResultCalculatedRow> rows)
    {
        return TimeSpan.FromTicks(rows.Sum(row => row.AnswerTime.Ticks));
    }

    private static string CreateQuestionKey(int questionIndex, string questionKey)
    {
        return $"{questionIndex}:{questionKey}";
    }

    private string? ValidateTeacherAccess(LoginState? loginState, ClassContext currentClass)
    {
        if (loginState is null)
        {
            return "Sign in as the teacher to view quiz results.";
        }

        if (!loginState.IsTeacher)
        {
            return "Only teachers can view quiz results.";
        }

        if (!string.Equals(loginState.ClassCode, currentClass.Code, StringComparison.OrdinalIgnoreCase))
        {
            return "Sign in as the teacher for this class to view quiz results.";
        }

        var teacher = teacherOptions.Value;

        return string.Equals(loginState.UserName, teacher.UserName, StringComparison.OrdinalIgnoreCase)
            ? null
            : "Teacher login is required to view quiz results.";
    }

    private sealed record QuizResultAnswerRow(
        int StudentId,
        string StudentDisplayName,
        int QuestionIndex,
        string QuestionKey,
        string QuestionText,
        string CorrectAnswer,
        string Answer,
        DateTime StartedAtUtc,
        DateTime? EndedAtUtc,
        bool IsCorrect);

    private sealed record QuizResultCalculatedRow(
        QuizResultAnswerRow Answer,
        TimeSpan AnswerTime);
}
