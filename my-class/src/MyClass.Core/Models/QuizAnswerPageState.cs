namespace MyClass.Core.Models;

public sealed record QuizAnswerPageState(
    bool HasInProgressAnswer,
    bool AlreadyAnswered,
    bool FailedNoAnswer,
    string Message,
    string QuizTitle,
    string? QuestionKey,
    string? QuestionTitle,
    int? QuestionIndex,
    int? QuestionCount,
    bool CurrentQuestionIsInProgress,
    TimeSpan CurrentQuestionRemaining,
    IReadOnlyList<string> AnswerChoices);


