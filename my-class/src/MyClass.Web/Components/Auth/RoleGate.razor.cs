using Microsoft.AspNetCore.Components;

namespace MyClass.Web.Components.Auth;

public partial class RoleGate
{
    [Parameter]
    public bool RequireTeacher { get; set; }

    [Parameter]
    public string DeniedMessage { get; set; } = "You do not have access to this page.";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private bool _loadedLoginState;

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

        var state = LoginStateService.Current ?? await SessionStorage.GetLoginStateAsync();
        LoginStateService.Set(state);
        _loadedLoginState = true;
        StateHasChanged();
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