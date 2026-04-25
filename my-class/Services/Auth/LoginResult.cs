namespace MyClass.Services.Auth;

public sealed record LoginResult(
    bool Succeeded,
    LoginState? State,
    string Message)
{
    public static LoginResult Success(LoginState state) => new(true, state, string.Empty);

    public static LoginResult Failure(string message) => new(false, null, message);
}
