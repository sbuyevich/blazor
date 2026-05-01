namespace MyClass.Core.Models;

public sealed record QuizSummary(
    string Path,
    string Name,
    string Title,
    int TimeLimitSeconds);
