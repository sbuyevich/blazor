namespace MyClass.Services.Students;

public sealed record StudentListItem(
    int Id,
    string UserName,
    string FirstName,
    string LastName,
    string DisplayName,
    DateTime CreatedAtUtc);
