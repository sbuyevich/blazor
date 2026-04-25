namespace MyClass.Services.BrowserStorage;

public interface ISessionStorageService
{
    ValueTask<T?> GetAsync<T>(string key);

    ValueTask SetAsync<T>(string key, T value);

    ValueTask RemoveAsync(string key);
}
