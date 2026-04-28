namespace MyClass.Core.Models;

public sealed record QuizAnswerPageState(
    bool HasInProgressAnswer,
    bool AlreadyAnswered,
    bool FailedNoAnswer,
    string Message,
    string? QuestionKey,
    string? QuestionTitle,
    IReadOnlyList<string> AnswerChoices);


