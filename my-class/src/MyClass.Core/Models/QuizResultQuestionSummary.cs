namespace MyClass.Core.Models;

public sealed record QuizResultQuestionSummary(
    int QuestionIndex,
    string QuestionKey,
    string QuestionText,
    int CorrectCount,
    int IncorrectCount,
    double PercentCorrect,
    TimeSpan TotalAnswerTime);
