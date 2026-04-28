namespace MyClass.Core.Services;

public sealed record QuizActionResult(
    bool Succeeded,
    string Message)
{
    public static QuizActionResult Success(string message) => new(true, message);

    public static QuizActionResult Failure(string message) => new(false, message);
}


