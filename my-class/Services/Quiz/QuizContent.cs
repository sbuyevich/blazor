namespace MyClass.Services.Quiz;

public sealed record QuizContent(
    string Title,
    IReadOnlyList<QuizQuestionContent> Questions);
