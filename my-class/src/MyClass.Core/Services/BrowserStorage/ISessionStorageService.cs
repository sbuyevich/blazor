using MyClass.Core.Services.Auth;

namespace MyClass.Core.Services.BrowserStorage;

public interface ISessionStorageService
{
    ValueTask<T?> GetAsync<T>(string key);

    ValueTask SetAsync<T>(string key, T value);

    ValueTask RemoveAsync(string key);

    ValueTask<LoginState?> GetLoginStateAsync();

    ValueTask SetLoginStateAsync(LoginState state);

    ValueTask RemoveLoginStateAsync();

    ValueTask<string?> GetClassCodeAsync();

    ValueTask SetClassCodeAsync(string classCode);

    ValueTask RemoveClassCodeAsync();
}


