using MyClass.Core.Models;

namespace  MyClass.Core.Services;

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


