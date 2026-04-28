using ClassContextModel =  MyClass.Core.Services.ClassContext;

namespace MyClass.Core.Services;

public interface IQuizNotificationService
{
    Task NotifyQuizStateChangedAsync(
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);
}
