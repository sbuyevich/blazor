namespace MyClass.Core.Models;

public sealed record SchoolClassActionResult(
    bool Succeeded,
    string Message,
    int? EntityId = null)
{
    public static SchoolClassActionResult Success(string message, int? entityId = null) =>
        new(true, message, entityId);

    public static SchoolClassActionResult Failure(string message) =>
        new(false, message);
}
