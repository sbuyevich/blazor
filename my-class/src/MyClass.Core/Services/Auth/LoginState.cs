namespace MyClass.Core.Services.Auth;

public sealed record LoginState(
    string UserName,
    bool IsTeacher,
    string ClassCode,
    string? DisplayName = null);


