namespace MyClass.Core.Models;

public sealed record QuizContent(
    string Name,
    string Title,
    int TimeLimitSeconds,
    IReadOnlyList<QuizQuestionContent> Questions);


