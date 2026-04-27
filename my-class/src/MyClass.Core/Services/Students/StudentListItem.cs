namespace MyClass.Core.Services.Students;

public sealed record StudentListItem(
    int Id,
    string UserName,
    string FirstName,
    string LastName,
    string DisplayName,
    bool IsActive,
    DateTime CreatedAtUtc);


