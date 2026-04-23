namespace student_projects.Models;

public sealed class AppUser
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public List<StudentProject> OwnedProjects { get; set; } = [];
}
