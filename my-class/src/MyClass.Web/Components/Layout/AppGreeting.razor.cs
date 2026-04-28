namespace MyClass.Web.Components.Layout;

public partial class AppGreeting
{
    private string GreetingText
    {
        get
        {
            var state = LoginStateService.Current;

            if (state is null)
            {
                return string.Empty;
            }

            var displayName = string.IsNullOrWhiteSpace(state.DisplayName)
                ? state.UserName
                : state.DisplayName;

            return $"Hi, {displayName}";
        }
    }

    protected override void OnInitialized()
    {
        LoginStateService.Changed += OnLoginStateChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || LoginStateService.Current is not null)
        {
            return;
        }

        var state = await SessionStorage.GetLoginStateAsync();

        if (state is not null)
        {
            LoginStateService.Set(state);
        }
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