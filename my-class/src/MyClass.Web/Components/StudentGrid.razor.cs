using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyClass.Core.Models;

namespace MyClass.Web.Components;

public partial class StudentGrid
{
    [Parameter, EditorRequired]
    public ClassContext CurrentClass { get; set; } = null!;

    [Parameter]
    public EventCallback<Result<IReadOnlyList<StudentListItem>>> StudentsLoaded { get; set; }

    private Result<IReadOnlyList<StudentListItem>>? _studentsResult;
    private LoginState? _loginState;
    private string? _searchText;
    private bool _activeOnly;
    private int? _loadedClassId;
    private int? _pendingClassId;
    private bool _isLoading = true;
    private bool _isRemoving;
    private bool _isResetting;

    private string EmptyStateText
    {
        get
        {
            if (_activeOnly && string.IsNullOrWhiteSpace(_searchText))
            {
                return "No active students are in this class yet.";
            }

            return string.IsNullOrWhiteSpace(_searchText)
                ? "No students are registered for this class yet."
                : "No students match those filters.";
        }
    }

    private IReadOnlyList<StudentListItem> StudentRows =>
        _studentsResult?.Value ?? [];

    protected override void OnParametersSet()
    {
        if (_loadedClassId == CurrentClass.ClassId)
        {
            return;
        }

        _pendingClassId = CurrentClass.ClassId;
        _isLoading = true;
        _studentsResult = null;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_pendingClassId != CurrentClass.ClassId)
        {
            return;
        }

        _pendingClassId = null;
        await LoadStudentsAsync(CurrentClass);
        StateHasChanged();
    }

    private async Task LoadStudentsAsync(ClassContext currentClass)
    {
        _loadedClassId = currentClass.ClassId;

        _loginState = LoginStateService.Current;

        if (_loginState is null)
        {
            _loginState = await SessionStorage.GetLoginStateAsync();
            LoginStateService.Set(_loginState);
        }

        _studentsResult = await StudentService.GetStudentsForClassAsync(_loginState, currentClass, _searchText, _activeOnly);
        _isLoading = false;

        await StudentsLoaded.InvokeAsync(_studentsResult);
    }

    private async Task HandleSearchChangedAsync(string value)
    {
        _searchText = value;
        _isLoading = true;

        await LoadStudentsAsync(CurrentClass);
    }

    private async Task HandleActiveOnlyChangedAsync(bool value)
    {
        _activeOnly = value;
        _isLoading = true;

        await LoadStudentsAsync(CurrentClass);
    }

    private async Task ConfirmRemoveStudentAsync(StudentListItem student)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            "Remove student",
            $"Remove {student.DisplayName}? This will delete the student account and they will need to register again.",
            yesText: "Remove",
            cancelText: "Cancel");

        if (confirmed != true)
        {
            return;
        }

        _isRemoving = true;

        var result = await StudentService.RemoveStudentFromClassAsync(
            _loginState,
            CurrentClass,
            student.Id);

        _isRemoving = false;

        if (!result.Succeeded)
        {
            Snackbar.Add(result.Message, Severity.Error);
            return;
        }

        _isLoading = true;
        await LoadStudentsAsync(CurrentClass);
    }

    private async Task ResetActiveStudentsAsync()
    {
        _isResetting = true;

        var result = await StudentService.ResetStudentsActiveStateAsync(
            _loginState,
            CurrentClass);

        _isResetting = false;

        if (!result.Succeeded)
        {
            Snackbar.Add(result.Message, Severity.Error);
            return;
        }

        _isLoading = true;
        await LoadStudentsAsync(CurrentClass);
    }
}
