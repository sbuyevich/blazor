namespace MyClass.Data.Entities;

public sealed class Student
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Class Class { get; set; } = null!;
}
