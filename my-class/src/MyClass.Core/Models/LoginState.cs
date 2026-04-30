namespace MyClass.Core.Models;

public sealed record LoginState(
    string UserName,
    bool IsTeacher,
    string ClassCode,
    string? DisplayName = null);


