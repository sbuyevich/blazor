namespace MyClass.Core.Services.Auth;

public interface ILoginStateService
{
    LoginState? Current { get; }

    event Action? Changed;

    void Set(LoginState? state);
}


