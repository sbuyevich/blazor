using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IQuizContentService
{
    Task<QuizContentResult> LoadQuizAsync(CancellationToken cancellationToken = default);

    Task<QuizImageResult> LoadQuestionImageAsync(
        string questionKey,
        CancellationToken cancellationToken = default);
}


