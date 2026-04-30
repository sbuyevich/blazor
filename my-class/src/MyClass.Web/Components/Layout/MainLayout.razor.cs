using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using MyClass.Core.Models;

namespace MyClass.Web.Components.Layout;

public partial class MainLayout
{
    private bool _drawerOpen = true;
    private bool _sessionStorageChecked;

    private ClassContext? CurrentClass => ClassContextState.CurrentClass;

    private string? ClassMessage =>
        ClassContextState.Result is { IsLoaded: false } result
            ? result.Message
            : null;

    private readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#2563eb",
            Secondary = "#0f766e",
            AppbarBackground = "#2563eb",
            Background = "#f8fafc",
            Surface = "#ffffff"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "6px",
            DrawerWidthLeft = "240px"
        }
    };

    protected override async Task OnInitializedAsync()
    {
        ClassContextState.Changed += OnClassContextChanged;
        Navigation.LocationChanged += OnLocationChanged;

        var classCode = ClassContextService.GetClassCodeFromUri(Navigation.Uri);

        if (!string.IsNullOrWhiteSpace(classCode))
        {
            await LoadClassContextAsync(classCode);
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _sessionStorageChecked = true;
        var classCode = ClassContextService.GetClassCodeFromUri(Navigation.Uri);

        if (!string.IsNullOrWhiteSpace(classCode))
        {
            await PersistResolvedClassCodeAsync();
            return;
        }

        var storedClassCode = await SessionStorage.GetClassCodeAsync();

        if (string.IsNullOrWhiteSpace(storedClassCode))
        {
            await LoadClassContextAsync(null);
        }
        else
        {
            await LoadClassContextAsync(storedClassCode);
        }
    }

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        await InvokeAsync(async () =>
        {
            var classCode = ClassContextService.GetClassCodeFromUri(args.Location);

            if (!string.IsNullOrWhiteSpace(classCode))
            {
                await LoadClassContextAsync(classCode);
                await PersistResolvedClassCodeAsync();
            }
            else if (CurrentClass is null && _sessionStorageChecked)
            {
                await LoadClassContextAsync(null);
            }

            StateHasChanged();
        });
    }

    private async Task LoadClassContextAsync(string? classCode)
    {
        var result = await ClassContextService.ResolveAsync(classCode);
        ClassContextState.Set(result);
    }

    private async Task PersistResolvedClassCodeAsync()
    {
        if (CurrentClass is not null)
        {
            await SessionStorage.SetClassCodeAsync(CurrentClass.Code);
        }
    }

    private void OnClassContextChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    private bool IsPublicAuthRoute()
    {
        var path = Navigation.ToBaseRelativePath(Navigation.Uri)
            .Split('?', '#')[0]
            .Trim('/')
            .ToLowerInvariant();

        return path is "" or "login" or "register";
    }

    public void Dispose()
    {
        ClassContextState.Changed -= OnClassContextChanged;
        Navigation.LocationChanged -= OnLocationChanged;
    }
}