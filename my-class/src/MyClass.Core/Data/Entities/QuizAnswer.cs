namespace MyClass.Core.Data.Entities;

public sealed class QuizAnswer
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentUserName { get; set; } = string.Empty;

    public string StudentFirstName { get; set; } = string.Empty;

    public string StudentLastName { get; set; } = string.Empty;

    public string StudentDisplayName { get; set; } = string.Empty;

    public string QuestionKey { get; set; } = string.Empty;

    public int QuestionIndex { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string CorrectAnswer { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public DateTime StartedAtUtc { get; set; }

    public DateTime? EndedAtUtc { get; set; }

    public DateTime? AnswerRevealedAtUtc { get; set; }

    public bool IsCorrect { get; set; }

    public Student Student { get; set; } = null!;
}


