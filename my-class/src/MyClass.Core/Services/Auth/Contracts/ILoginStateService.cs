using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface ILoginStateService
{
    LoginState? Current { get; }

    event Action? Changed;

    void Set(LoginState? state);
}


