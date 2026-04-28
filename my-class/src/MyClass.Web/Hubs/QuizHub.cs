using Microsoft.AspNetCore.SignalR;

namespace MyClass.Web.Hubs;

public sealed class QuizHub : Hub
{
    public const string Route = "/hubs/quiz";

    public const string QuizStateChangedMethod = "QuizStateChanged";

    public async Task JoinClass(string classCode)
    {
        var groupName = CreateClassGroupName(classCode);

        if (groupName is null)
        {
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public static string? CreateClassGroupName(string? classCode)
    {
        var normalizedClassCode = classCode?.Trim().ToLowerInvariant();

        return string.IsNullOrWhiteSpace(normalizedClassCode)
            ? null
            : $"class:{normalizedClassCode}";
    }
}
