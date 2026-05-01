namespace MyClass.Core.Models;

public sealed record SchoolClassClassItem(
    int Id,
    int SchoolId,
    string Name,
    string Code,
    int StudentCount);
