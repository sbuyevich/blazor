namespace MyClass.Services.Auth;

public sealed record LoginState(
    string UserName,
    bool IsTeacher,
    string ClassCode);
