namespace MyClass.Core.Services.Auth;

public sealed class LoginStateService : ILoginStateService
{
    public LoginState? Current { get; private set; }

    public event Action? Changed;

    public void Set(LoginState? state)
    {
        Current = state;
        Changed?.Invoke();
    }
}


