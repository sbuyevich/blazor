using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyClass.Core.Models;
using MyClass.Core.Services;

namespace MyClass.Web.Components.Auth;

public partial class LoginForm
{
    [Parameter, EditorRequired]
    public ClassContext CurrentClass { get; set; } = null!;

    [Parameter]
    public bool ShowRegisterLink { get; set; } = true;

    [Parameter]
    public EventCallback<LoginState> Succeeded { get; set; }

    [Parameter]
    public EventCallback RegisterRequested { get; set; }

    private readonly LoginModel _login = new();
    private MudForm? _loginForm;
    private bool _isSubmitting;
    private string? _message;
    private Severity _messageSeverity = Severity.Error;

    private async Task HandleLoginAsync()
    {
        if (_loginForm is null)
        {
            return;
        }

        await _loginForm.ValidateAsync();

        if (!_loginForm.IsValid)
        {
            return;
        }

        _isSubmitting = true;
        _message = null;

        var result = await AuthService.LoginAsync(
            _login.UserName,
            _login.Password,
            isTeacher: false,
            CurrentClass.Code);

        if (!result.Succeeded || result.State is null)
        {
            await SessionStorage.RemoveLoginStateAsync();
            LoginStateService.Set(null);
            _messageSeverity = Severity.Error;
            _message = result.Message;
            _isSubmitting = false;
            return;
        }

        await SessionStorage.SetLoginStateAsync(result.State);
        LoginStateService.Set(result.State);
        _isSubmitting = false;

        await Succeeded.InvokeAsync(result.State);
    }

    private Task RequestRegisterAsync()
    {
        return RegisterRequested.InvokeAsync();
    }

    private sealed class LoginModel
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}