using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MyClass.Core.Models;

namespace MyClass.Web.Components;

public partial class ClassContextView
{
    [Parameter]
    public RenderFragment<ClassContext>? ChildContent { get; set; }

    private Result<ClassContext>? _result;

    protected override async Task OnInitializedAsync()
    {
        Navigation.LocationChanged += OnLocationChanged;
        await LoadClassContextAsync();
    }

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        await InvokeAsync(async () =>
        {
            await LoadClassContextAsync();
            StateHasChanged();
        });
    }

    private async Task LoadClassContextAsync()
    {
        var classCode = ClassContextService.GetClassCodeFromUri(Navigation.Uri);
        _result = await ClassContextService.ResolveAsync(classCode);
    }

    public void Dispose()
    {
        Navigation.LocationChanged -= OnLocationChanged;
    }
}
