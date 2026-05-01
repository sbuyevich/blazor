using Microsoft.AspNetCore.Components;
using MyClass.Core.Models;

namespace  MyClass.Web.Pages;

public partial class Login
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        var state = LoginStateService.Current ?? await SessionStorage.GetLoginStateAsync();

        if (state is null)
        {
            return;
        }

        LoginStateService.Set(state);
        Navigation.NavigateTo("/home");
    }

    private Task HandleSucceededAsync(LoginState state)
    {
        Navigation.NavigateTo("/home");

        return Task.CompletedTask;
    }

    private void GoToRegister()
    {
        Navigation.NavigateTo("/register");
    }
}