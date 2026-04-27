using MyClass.Services.Auth;
using ClassContextModel = MyClass.Services.ClassContext.ClassContext;

namespace MyClass.Services.Quiz;

public interface IQuizAnswerService
{
    Task<QuizAnswerPageStateResult> GetAnswerPageStateAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> SubmitAnswerAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        string selectedAnswer,
        CancellationToken cancellationToken = default);
}
