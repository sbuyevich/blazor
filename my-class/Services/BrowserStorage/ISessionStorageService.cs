using MyClass.Services.Auth;

namespace MyClass.Services.BrowserStorage;

public interface ISessionStorageService
{
    ValueTask<T?> GetAsync<T>(string key);

    ValueTask SetAsync<T>(string key, T value);

    ValueTask RemoveAsync(string key);

    ValueTask<LoginState?> GetLoginStateAsync();

    ValueTask SetLoginStateAsync(LoginState state);

    ValueTask RemoveLoginStateAsync();
}
