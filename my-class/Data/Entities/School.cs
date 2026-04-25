namespace MyClass.Data.Entities;

public sealed class School
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Class> Classes { get; set; } = [];
}
