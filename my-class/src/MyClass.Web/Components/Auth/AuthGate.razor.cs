using Microsoft.AspNetCore.Components;
using MyClass.Core.Models;
using MyClass.Core.Services;
using  MyClass.Core.Services;

namespace MyClass.Web.Components.Auth;

public partial class AuthGate
{
    [Parameter]
    public ClassContext? CurrentClass { get; set; }

    private bool _loadedLoginState;
    private bool _showRegistration;

    private bool ShouldShowGate =>
        CurrentClass is not null &&
        (!_loadedLoginState || LoginStateService.Current is null);

    protected override void OnInitialized()
    {
        LoginStateService.Changed += OnLoginStateChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        var state = await SessionStorage.GetLoginStateAsync();
        LoginStateService.Set(state);
        _loadedLoginState = true;
        StateHasChanged();
    }

    private async Task HandleAuthSucceededAsync(LoginState state)
    {
        _showRegistration = false;
        await InvokeAsync(() => Navigation.NavigateTo("/home"));
    }

    private void ShowRegistration()
    {
        _showRegistration = true;
    }

    private void ShowLogin()
    {
        _showRegistration = false;
    }

    private void OnLoginStateChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        LoginStateService.Changed -= OnLoginStateChanged;
    }
}