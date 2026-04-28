namespace MyClass.Core.Services;

public sealed record QuizTeacherQuestionState(
    int QuestionIndex,
    int QuestionCount,
    string QuestionKey,
    string Title,
    int TimeoutSeconds,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    bool IsInProgress,
    TimeSpan Remaining);


