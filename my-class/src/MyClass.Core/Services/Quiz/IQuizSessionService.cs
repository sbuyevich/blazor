using MyClass.Core.Services.Auth;
using ClassContextModel = MyClass.Core.Services.ClassContext.ClassContext;

namespace MyClass.Core.Services.Quiz;

public interface IQuizSessionService
{
    Task<QuizTeacherStateResult> GetTeacherStateAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> StartQuestionAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> RestartQuizAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> FinishCurrentQuestionAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> MoveNextQuestionAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);
}


