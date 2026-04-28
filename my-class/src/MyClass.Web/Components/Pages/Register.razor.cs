using Microsoft.AspNetCore.Components;
using MyClass.Core.Services;
using  MyClass.Core.Services;

namespace MyClass.Web.Components.Pages;

public partial class Register
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

    private void GoToLogin()
    {
        Navigation.NavigateTo("/login");
    }
}