namespace MyClass.Services.Quiz;

public sealed record QuizStudentAnswerStatus(
    int StudentId,
    string UserName,
    string DisplayName,
    bool HasAnswered,
    bool FailedNoAnswer);
