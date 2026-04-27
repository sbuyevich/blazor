namespace MyClass.Services.Quiz;

public sealed record QuizQuestionContent(
    string Key,
    int Index,
    string Title,
    int TimeoutSeconds,
    string CorrectAnswer,
    string ImageReference);
