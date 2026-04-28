namespace MyClass.Core.Services;

public sealed record QuizStudentAnswerStatus(
    int StudentId,
    string UserName,
    string DisplayName,
    bool HasAnswered,
    bool FailedNoAnswer);


