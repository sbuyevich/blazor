using Microsoft.JSInterop;
using MyClass.Core.Models;
using System.Text.Json;

namespace  MyClass.Core.Services;

public sealed class SessionStorageService(IJSRuntime jsRuntime) : ISessionStorageService
{
    private const string LoginStateKey = "my-class.loginState";
    private const string ClassCodeKey = "my-class.classCode";
    private const string SelectedQuizPathKey = "my-class.selectedQuizPath";
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

    public ValueTask<string?> GetClassCodeAsync()
    {
        return GetAsync<string>(ClassCodeKey);
    }

    public ValueTask SetClassCodeAsync(string classCode)
    {
        return SetAsync(ClassCodeKey, classCode);
    }

    public ValueTask RemoveClassCodeAsync()
    {
        return RemoveAsync(ClassCodeKey);
    }

    public ValueTask<string?> GetSelectedQuizPathAsync()
    {
        return GetAsync<string>(SelectedQuizPathKey);
    }

    public ValueTask SetSelectedQuizPathAsync(string quizPath)
    {
        return SetAsync(SelectedQuizPathKey, quizPath);
    }

    public ValueTask RemoveSelectedQuizPathAsync()
    {
        return RemoveAsync(SelectedQuizPathKey);
    }
}


