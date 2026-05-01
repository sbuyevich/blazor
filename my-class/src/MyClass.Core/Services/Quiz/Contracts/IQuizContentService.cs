using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IQuizContentService
{
    Task<Result<IReadOnlyList<QuizSummary>>> LoadAvailableQuizzesAsync(
        CancellationToken cancellationToken = default);

    Task<Result<QuizContent>> LoadQuizAsync(CancellationToken cancellationToken = default);

    Task<Result<QuizContent>> LoadQuizAsync(
        string? quizFolderPath,
        CancellationToken cancellationToken = default);

    Task<Result<string>> LoadQuestionImageAsync(
        string questionKey,
        string? quizFolderPath = null,
        CancellationToken cancellationToken = default);

    Task<Result<string>> LoadAnswerImageAsync(
        string questionKey,
        string? quizFolderPath = null,
        CancellationToken cancellationToken = default);
}


