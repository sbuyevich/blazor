namespace MyClass.Data.Entities;

public sealed class QuizAnswer
{
    public int Id { get; set; }

    public int QuizSessionQuestionId { get; set; }

    public int StudentId { get; set; }

    public QuizAnswerStatus Status { get; set; } = QuizAnswerStatus.InProgress;

    public int? SelectedAnswer { get; set; }

    public bool? IsCorrect { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? SubmittedAtUtc { get; set; }

    public QuizSessionQuestion QuizSessionQuestion { get; set; } = null!;

    public Student Student { get; set; } = null!;
}
