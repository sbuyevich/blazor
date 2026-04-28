
using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IQuizAnswerService
{
    Task<QuizAnswerPageStateResult> GetAnswerPageStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> SubmitAnswerAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string selectedAnswer,
        CancellationToken cancellationToken = default);
}


