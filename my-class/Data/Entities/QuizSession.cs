namespace MyClass.Data.Entities;

public sealed class QuizSession
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public string Title { get; set; } = string.Empty;

    public QuizSessionStatus Status { get; set; } = QuizSessionStatus.InProgress;

    public int ActiveQuestionIndex { get; set; }

    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAtUtc { get; set; }

    public Class Class { get; set; } = null!;

    public ICollection<QuizSessionQuestion> Questions { get; set; } = [];
}
