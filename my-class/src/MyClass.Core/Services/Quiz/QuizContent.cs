namespace MyClass.Core.Services.Quiz;

public sealed record QuizContent(
    string Title,
    int TimeLimitSeconds,
    IReadOnlyList<QuizQuestionContent> Questions);


