namespace MyClass.Core.Services;

public sealed record QuizContent(
    string Title,
    int TimeLimitSeconds,
    IReadOnlyList<QuizQuestionContent> Questions);


