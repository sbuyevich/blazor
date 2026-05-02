namespace MyClass.Core.Models;

public sealed record QuizResultStudentSummary(
    int StudentId,
    string StudentDisplayName,
    int CorrectCount,
    int IncorrectCount,
    double PercentCorrect,
    TimeSpan TotalAnswerTime);
