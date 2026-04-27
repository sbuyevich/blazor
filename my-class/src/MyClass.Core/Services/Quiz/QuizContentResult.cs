namespace MyClass.Core.Services.Quiz;

public sealed record QuizContentResult(
    bool Succeeded,
    string Message,
    QuizContent? Quiz)
{
    public static QuizContentResult Success(QuizContent quiz) => new(true, string.Empty, quiz);

    public static QuizContentResult Failure(string message) => new(false, message, null);
}


