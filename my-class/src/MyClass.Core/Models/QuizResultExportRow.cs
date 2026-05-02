namespace MyClass.Core.Models;

public sealed record QuizResultExportRow(
    string StudentDisplayName,
    string QuestionText,
    string CorrectAnswer,
    string Answer,
    TimeSpan AnswerTime,
    bool IsCorrect);
