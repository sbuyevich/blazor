using System.Text.Json;
using Microsoft.JSInterop;
using MyClass.Core.Services.Auth;

namespace MyClass.Core.Services.BrowserStorage;

public sealed class SessionStorageService(IJSRuntime jsRuntime) : ISessionStorageService
{
    private const string LoginStateKey = "my-class.loginState";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var json = await jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", key);

        return string.IsNullOrWhiteSpace(json)
            ? default
            : JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public async ValueTask SetAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, json);
    }

    public ValueTask RemoveAsync(string key)
    {
        return jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
    }

    public ValueTask<LoginState?> GetLoginStateAsync()
    {
        return GetAsync<LoginState>(LoginStateKey);
    }

    public ValueTask SetLoginStateAsync(LoginState state)
    {
        return SetAsync(LoginStateKey, state);
    }

    public ValueTask RemoveLoginStateAsync()
    {
        return RemoveAsync(LoginStateKey);
    }
}


