namespace MyClass.Core.Models;

public sealed record QuizAnswerPageState(
    bool HasInProgressAnswer,
    bool AlreadyAnswered,
    bool FailedNoAnswer,
    string Message,
    string QuizTitle,
    string? ActiveQuizPath,
    string? QuestionKey,
    string? QuestionTitle,
    int? QuestionIndex,
    int? QuestionCount,
    bool IsAnswerRevealed,
    bool? IsCorrect,
    DateTime? AnsweredAtUtc,
    TimeSpan? AnswerElapsed,
    string? RevealMessage,
    bool CurrentQuestionIsInProgress,
    TimeSpan CurrentQuestionRemaining,
    IReadOnlyList<QuizQuestionProgressItem> QuestionProgress,
    IReadOnlyList<string> AnswerChoices);

public sealed record QuizQuestionProgressItem(
    int QuestionIndex,
    bool IsCurrent,
    bool IsBold,
    QuizQuestionProgressResult Result);

public enum QuizQuestionProgressResult
{
    Neutral,
    Correct,
    Incorrect,
    Missed
}


