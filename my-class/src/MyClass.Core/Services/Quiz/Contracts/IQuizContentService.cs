using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IQuizContentService
{
    Task<Result<QuizContent>> LoadQuizAsync(CancellationToken cancellationToken = default);

    Task<Result<string>> LoadQuestionImageAsync(
        string questionKey,
        CancellationToken cancellationToken = default);
}


