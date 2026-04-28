namespace MyClass.Core.Models;

public sealed record StudentActionResult(
    bool Succeeded,
    string Message)
{
    public static StudentActionResult Success(string message) => new(true, message);

    public static StudentActionResult Failure(string message) => new(false, message);
}


