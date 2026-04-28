using Microsoft.AspNetCore.SignalR;
using MyClass.Core.Services.ClassContext;
using MyClass.Core.Services.Quiz;

namespace MyClass.Web.Hubs;

public sealed class SignalRQuizNotificationService(IHubContext<QuizHub> hubContext) : IQuizNotificationService
{
    public async Task NotifyQuizStateChangedAsync(
        ClassContext currentClass,
        CancellationToken cancellationToken = default)
    {
        var groupName = QuizHub.CreateClassGroupName(currentClass.Code);

        if (groupName is null)
        {
            return;
        }

        await hubContext.Clients
            .Group(groupName)
            .SendAsync(QuizHub.QuizStateChangedMethod, cancellationToken);
    }
}
