namespace MyClass.Core.Models;

public sealed record StudentListItem(
    int Id,
    string UserName,
    string FirstName,
    string LastName,
    string DisplayName,
    bool IsActive,
    DateTime CreatedAtUtc);


