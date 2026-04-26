namespace MyClass.Data.Entities;

public sealed class QuizSessionQuestion
{
    public int Id { get; set; }

    public int QuizSessionId { get; set; }

    public int QuestionIndex { get; set; }

    public string QuestionKey { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; }

    public int CorrectAnswer { get; set; }

    public QuizQuestionStatus Status { get; set; } = QuizQuestionStatus.InProgress;

    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAtUtc { get; set; }

    public QuizSession QuizSession { get; set; } = null!;
}
