namespace MyClass.Core.Models;

public sealed record QuizTeacherQuestionState(
    int QuestionIndex,
    int QuestionCount,
    string QuestionKey,
    string Title,
    int TimeoutSeconds,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    DateTime? AnswerRevealedAtUtc,
    bool IsInProgress,
    bool IsAnswerRevealed,
    TimeSpan Remaining);


