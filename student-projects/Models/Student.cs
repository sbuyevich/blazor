namespace student_projects.Models;

public sealed class Student
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int? ClassId { get; set; }

    public Class? Class { get; set; }

    public List<StudentProject> OwnedProjects { get; set; } = [];
}
