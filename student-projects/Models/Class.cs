namespace student_projects.Models;

public sealed class Class
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int ShoolId { get; set; }

    public School School { get; set; } = null!;

    public List<Student> Students { get; set; } = [];
}
