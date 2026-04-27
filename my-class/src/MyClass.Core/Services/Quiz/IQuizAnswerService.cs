using MyClass.Core.Services.Auth;
using ClassContextModel = MyClass.Core.Services.ClassContext.ClassContext;

namespace MyClass.Core.Services.Quiz;

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


