namespace student_projects.Models;

public sealed class StudentProject
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int OwnerUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public AppUser Owner { get; set; } = null!;
}
