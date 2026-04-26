namespace MyClass.Services.Quiz;

public interface IQuizContentService
{
    Task<QuizContentResult> LoadQuizAsync(CancellationToken cancellationToken = default);

    Task<QuizImageResult> LoadQuestionImageAsync(
        string questionKey,
        CancellationToken cancellationToken = default);
}
