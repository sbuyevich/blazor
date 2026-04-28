namespace MyClass.Core.Services;

public interface IQuizNotificationService
{
    Task NotifyQuizStateChangedAsync(
        ClassContext currentClass,
        CancellationToken cancellationToken = default);
}
