using Microsoft.AspNetCore.SignalR;
using  MyClass.Core.Services;
using MyClass.Core.Services;

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
