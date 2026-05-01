using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MyClass.Web.Components.SchoolClass;

public partial class ClassEditDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public string Name { get; set; } = string.Empty;

    [Parameter]
    public string Code { get; set; } = string.Empty;

    [Parameter]
    public string SaveText { get; set; } = "Save";

    private string _name = string.Empty;
    private string _code = string.Empty;

    private bool IsSubmitDisabled =>
        string.IsNullOrWhiteSpace(_name) ||
        string.IsNullOrWhiteSpace(_code);

    protected override void OnInitialized()
    {
        _name = Name;
        _code = Code;
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private void Submit()
    {
        var normalizedName = _name.Trim();
        var normalizedCode = _code.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName) ||
            string.IsNullOrWhiteSpace(normalizedCode))
        {
            return;
        }

        MudDialog.Close(DialogResult.Ok(new ClassDialogInput(normalizedName, normalizedCode)));
    }
}

public sealed record ClassDialogInput(string Name, string Code);
