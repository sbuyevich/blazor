using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyClass.Core.Services.Auth;
using MyClass.Core.Services.ClassContext;

namespace MyClass.Web.Components.Auth;

public partial class StudentRegistrationForm
{
    [Parameter, EditorRequired]
    public ClassContext CurrentClass { get; set; } = null!;

    [Parameter]
    public bool ShowLoginLink { get; set; } = true;

    [Parameter]
    public EventCallback<LoginState> Succeeded { get; set; }

    [Parameter]
    public EventCallback LoginRequested { get; set; }

    private readonly StudentRegistrationModel _registration = new();
    private MudForm? _registrationForm;
    private bool _isSubmitting;
    private string? _message;
    private Severity _messageSeverity = Severity.Error;

    private async Task HandleRegistrationAsync()
    {
        if (_registrationForm is null)
        {
            return;
        }

        await _registrationForm.ValidateAsync();

        if (!_registrationForm.IsValid)
        {
            return;
        }

        if (_registration.Password != _registration.ConfirmPassword)
        {
            _messageSeverity = Severity.Error;
            _message = "Passwords do not match.";
            return;
        }

        _isSubmitting = true;
        _message = null;

        var result = await AuthService.RegisterStudentAsync(
            _registration.UserName,
            _registration.FirstName,
            _registration.LastName,
            _registration.Password,
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

    private Task RequestLoginAsync()
    {
        return LoginRequested.InvokeAsync();
    }

    private sealed class StudentRegistrationModel
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}