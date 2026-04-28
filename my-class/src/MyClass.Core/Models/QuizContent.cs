namespace MyClass.Core.Models;

public sealed record QuizContent(
    string Title,
    int TimeLimitSeconds,
    IReadOnlyList<QuizQuestionContent> Questions);


