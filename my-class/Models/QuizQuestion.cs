namespace MyClass.Models;

public sealed class QuizQuestion
{
    public string Folder { get; set; } = string.Empty;

    public string Question { get; set; } = string.Empty;

    public int? TimeLimitSeconds { get; set; } 

    public string StudentAnswer { get; set; } = string.Empty;

    public string CorrectAnswer { get; set; } = string.Empty;
        
}
