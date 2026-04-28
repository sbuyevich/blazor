namespace MyClass.Core.Models;

public sealed record QuizQuestionContent(
    string Key,
    int Index,
    string Title,
    int TimeoutSeconds,
    int AnswerCount,
    string CorrectAnswer,
    string ImageReference);


