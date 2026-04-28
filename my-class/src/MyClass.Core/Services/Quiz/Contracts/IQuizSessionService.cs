using MyClass.Core.Models;
using ClassContext = MyClass.Core.Models.ClassContext;

namespace MyClass.Core.Services;

public interface IQuizSessionService
{
    Task<QuizTeacherStateResult> GetTeacherStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> StartQuestionAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> RestartQuizAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> FinishCurrentQuestionAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);

    Task<QuizActionResult> MoveNextQuestionAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);
}


