namespace MyClass.Core.Data.Entities;

public sealed class Class
{
    public int Id { get; set; }

    public int SchoolId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public School School { get; set; } = null!;

    public ICollection<Student> Students { get; set; } = [];
}


