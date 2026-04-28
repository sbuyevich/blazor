namespace MyClass.Core.Services;

public sealed record LoginState(
    string UserName,
    bool IsTeacher,
    string ClassCode,
    string? DisplayName = null);


