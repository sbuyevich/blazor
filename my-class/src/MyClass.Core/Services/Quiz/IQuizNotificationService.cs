using ClassContextModel = MyClass.Core.Services.ClassContext.ClassContext;

namespace MyClass.Core.Services.Quiz;

public interface IQuizNotificationService
{
    Task NotifyQuizStateChangedAsync(
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);
}
