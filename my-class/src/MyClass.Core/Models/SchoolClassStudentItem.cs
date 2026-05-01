namespace MyClass.Core.Models;

public sealed record SchoolClassStudentItem(
    int Id,
    string UserName,
    string DisplayName,
    bool IsActive,
    DateTime CreatedAtUtc);
