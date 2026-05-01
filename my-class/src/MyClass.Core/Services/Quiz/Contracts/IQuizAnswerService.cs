
using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IQuizAnswerService
{
    Task<Result<QuizAnswerPageState>> GetAnswerPageStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> SubmitAnswerAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string selectedAnswer,
        CancellationToken cancellationToken = default);
}


