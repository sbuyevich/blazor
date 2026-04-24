namespace student_projects.Models;

public sealed class School
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<Class> Classes { get; set; } = [];
}
