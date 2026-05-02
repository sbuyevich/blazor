using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MyClass.Web.Components;

public partial class SchoolEditDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public string Name { get; set; } = string.Empty;

    [Parameter]
    public string SaveText { get; set; } = "Save";

    private string _name = string.Empty;

    protected override void OnInitialized()
    {
        _name = Name;
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private void Submit()
    {
        var normalizedName = _name.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return;
        }

        MudDialog.Close(DialogResult.Ok(new SchoolDialogInput(normalizedName)));
    }
}

public sealed record SchoolDialogInput(string Name);
