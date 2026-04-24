namespace student_projects.Models;

public sealed class Student
{
    public int Id { get; set; }

    public int? ClassId { get; set; }

    public Class? Class { get; set; }
}
