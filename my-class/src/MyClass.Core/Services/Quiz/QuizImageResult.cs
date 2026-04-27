namespace MyClass.Core.Services.Quiz;

public sealed record QuizImageResult(
    bool Succeeded,
    string Message,
    string? DataUri)
{
    public static QuizImageResult Success(string dataUri) => new(true, string.Empty, dataUri);

    public static QuizImageResult Failure(string message) => new(false, message, null);
}


