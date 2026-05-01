using MyClass.Core.Models;
using ClassContext = MyClass.Core.Models.ClassContext;

namespace MyClass.Core.Services;

public interface IQuizSessionService
{
    Task<Result<QuizTeacherState>> GetTeacherStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string? quizFolderPath = null,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> StartQuestionAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string? quizFolderPath = null,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> RestartQuizAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string? quizFolderPath = null,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> FinishCurrentQuestionAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string? quizFolderPath = null,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> ShowAnswerAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string? quizFolderPath = null,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> MoveNextQuestionAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string? quizFolderPath = null,
        CancellationToken cancellationToken = default);
}


