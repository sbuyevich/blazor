namespace MyClass.Core.Models;

public sealed record QuizStudentAnswerStatus(
    int StudentId,
    string UserName,
    string DisplayName,
    bool HasAnswered,
    bool FailedNoAnswer,
    DateTime? AnsweredAtUtc,
    TimeSpan? AnswerElapsed,
    bool? IsCorrect);


