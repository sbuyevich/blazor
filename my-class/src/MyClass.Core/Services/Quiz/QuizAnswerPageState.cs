namespace MyClass.Core.Services;

public sealed record QuizAnswerPageState(
    bool HasInProgressAnswer,
    bool AlreadyAnswered,
    bool FailedNoAnswer,
    string Message,
    IReadOnlyList<string> AnswerChoices);


