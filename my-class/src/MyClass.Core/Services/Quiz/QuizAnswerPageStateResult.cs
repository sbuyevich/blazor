namespace MyClass.Core.Services;

public sealed record QuizAnswerPageStateResult(
    bool Succeeded,
    string Message,
    QuizAnswerPageState? State)
{
    public static QuizAnswerPageStateResult Success(QuizAnswerPageState state) => new(true, string.Empty, state);

    public static QuizAnswerPageStateResult Failure(string message) => new(false, message, null);
}


