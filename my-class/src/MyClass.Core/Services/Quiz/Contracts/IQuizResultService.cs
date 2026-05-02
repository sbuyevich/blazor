using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IQuizResultService
{
    Task<Result<QuizResultPageState>> GetResultPageStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);
}
