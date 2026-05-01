using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyClass.Core.Models;
using MyClass.Core.Services;
using MyClass.Web.Components.SchoolClass;

namespace  MyClass.Web.Pages;

public partial class SchoolClass
{
    [Inject]
    private ISchoolClassService SchoolClassService { get; set; } = null!;

    [Inject]
    private ILoginStateService LoginStateService { get; set; } = null!;

    [Inject]
    private ISessionStorageService SessionStorage { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    private readonly DialogOptions _dialogOptions = new()
    {
        CloseButton = true,
        FullWidth = true,
        MaxWidth = MaxWidth.Small
    };

    private Result<IReadOnlyList<SchoolClassSchoolItem>>? _schoolClassResult;
    private Result<IReadOnlyList<SchoolClassStudentItem>>? _studentResult;
    private IReadOnlyList<SchoolClassSchoolItem> _schools = [];
    private LoginState? _loginState;
    private int? _selectedSchoolId;
    private int? _selectedClassId;
    private string? _studentSearchText;
    private bool _isLoading = true;
    private bool _isLoadingStudents;
    private bool _isSaving;

    private IReadOnlyList<SchoolClassClassItem> SelectedClasses =>
        _schools.FirstOrDefault(school => school.Id == _selectedSchoolId)?.Classes ?? [];

    private IReadOnlyList<SchoolClassStudentItem> StudentRows =>
        _studentResult?.Value ?? [];

    private string StudentEmptyStateText =>
        string.IsNullOrWhiteSpace(_studentSearchText)
            ? "No students are registered for this class yet."
            : "No students match that display name.";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await LoadSchoolClassesAsync();
        StateHasChanged();
    }

    private async Task LoadLoginStateAsync()
    {
        _loginState = LoginStateService.Current ?? await SessionStorage.GetLoginStateAsync();

        if (_loginState is not null && LoginStateService.Current is null)
        {
            LoginStateService.Set(_loginState);
        }
    }

    private async Task LoadSchoolClassesAsync(int? preferredSchoolId = null, int? preferredClassId = null)
    {
        _isLoading = true;
        await LoadLoginStateAsync();

        _schoolClassResult = await SchoolClassService.GetSchoolClassesAsync(_loginState);
        _schools = _schoolClassResult.Succeeded && _schoolClassResult.Value is not null
            ? _schoolClassResult.Value
            : [];

        SelectSchoolAndClass(preferredSchoolId, preferredClassId);
        _isLoading = false;

        await LoadStudentsAsync();
    }

    private void SelectSchoolAndClass(int? preferredSchoolId, int? preferredClassId)
    {
        var schoolId = preferredSchoolId ?? _selectedSchoolId;
        var selectedSchool = _schools.FirstOrDefault(school => school.Id == schoolId) ?? _schools.FirstOrDefault();

        _selectedSchoolId = selectedSchool?.Id;

        if (selectedSchool is null)
        {
            _selectedClassId = null;
            return;
        }

        var classId = preferredClassId ?? _selectedClassId;
        var selectedClass = selectedSchool.Classes.FirstOrDefault(@class => @class.Id == classId) ??
            selectedSchool.Classes.FirstOrDefault();

        _selectedClassId = selectedClass?.Id;
    }

    private async Task LoadStudentsAsync()
    {
        if (!_selectedClassId.HasValue)
        {
            _studentResult = null;
            return;
        }

        _isLoadingStudents = true;
        await LoadLoginStateAsync();

        _studentResult = await SchoolClassService.GetStudentsForClassAsync(
            _loginState,
            _selectedClassId.Value,
            _studentSearchText);

        _isLoadingStudents = false;
    }

    private async Task HandleSchoolSelectedAsync(int? schoolId)
    {
        _selectedSchoolId = schoolId;
        _selectedClassId = SelectedClasses.FirstOrDefault()?.Id;
        _studentSearchText = null;

        await LoadStudentsAsync();
    }

    private async Task HandleClassSelectedAsync(int? classId)
    {
        _selectedClassId = classId;
        _studentSearchText = null;

        await LoadStudentsAsync();
    }

    private async Task HandleStudentSearchChangedAsync(string value)
    {
        _studentSearchText = value;
        await LoadStudentsAsync();
    }

    private async Task AddSchoolAsync()
    {
        var input = await ShowSchoolDialogAsync("Add school", string.Empty, "Add");

        if (input is null)
        {
            return;
        }

        await ExecuteSchoolClassActionAsync(
            () => SchoolClassService.CreateSchoolAsync(_loginState, input.Name),
            schoolId => LoadSchoolClassesAsync(schoolId, null));
    }

    private async Task EditSchoolAsync(SchoolClassSchoolItem school)
    {
        var input = await ShowSchoolDialogAsync("Edit school", school.Name, "Save");

        if (input is null)
        {
            return;
        }

        await ExecuteSchoolClassActionAsync(
            () => SchoolClassService.UpdateSchoolAsync(_loginState, school.Id, input.Name),
            _ => LoadSchoolClassesAsync(school.Id, _selectedClassId));
    }

    private async Task DeleteSchoolAsync(SchoolClassSchoolItem school)
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            "Delete school",
            $"Delete {school.Name}?",
            yesText: "Delete",
            cancelText: "Cancel");

        if (confirmed != true)
        {
            return;
        }

        if (!await ConfirmDeleteAsync())
        {
            return;
        }

        await ExecuteSchoolClassActionAsync(
            () => SchoolClassService.DeleteSchoolAsync(_loginState, school.Id),
            _ => LoadSchoolClassesAsync(null, null));
    }

    private async Task AddClassAsync()
    {
        if (!_selectedSchoolId.HasValue)
        {
            Snackbar.Add("Select a school before adding a class.", Severity.Warning);
            return;
        }

        var input = await ShowClassDialogAsync("Add class", string.Empty, string.Empty, "Add");

        if (input is null)
        {
            return;
        }

        await ExecuteSchoolClassActionAsync(
            () => SchoolClassService.CreateClassAsync(_loginState, _selectedSchoolId.Value, input.Name, input.Code),
            classId => LoadSchoolClassesAsync(_selectedSchoolId, classId));
    }

    private async Task EditClassAsync(SchoolClassClassItem classItem)
    {
        var input = await ShowClassDialogAsync("Edit class", classItem.Name, classItem.Code, "Save");

        if (input is null)
        {
            return;
        }

        await ExecuteSchoolClassActionAsync(
            () => SchoolClassService.UpdateClassAsync(_loginState, classItem.Id, input.Name, input.Code),
            _ => LoadSchoolClassesAsync(classItem.SchoolId, classItem.Id));
    }

    private async Task DeleteClassAsync(SchoolClassClassItem classItem)
    {
        var message = classItem.StudentCount > 0
            ? $"Delete {classItem.Name}? This will also delete {classItem.StudentCount} registered student account{(classItem.StudentCount == 1 ? string.Empty : "s")}."
            : $"Delete {classItem.Name}?";

        var confirmed = await DialogService.ShowMessageBoxAsync(
            "Delete class",
            message,
            yesText: "Delete",
            cancelText: "Cancel");

        if (confirmed != true)
        {
            return;
        }

        if (!await ConfirmDeleteAsync())
        {
            return;
        }

        await ExecuteSchoolClassActionAsync(
            () => SchoolClassService.DeleteClassAsync(_loginState, classItem.Id),
            _ => LoadSchoolClassesAsync(classItem.SchoolId, null));
    }

    private async Task RemoveStudentAsync(SchoolClassStudentItem student)
    {
        if (!_selectedClassId.HasValue)
        {
            return;
        }

        var confirmed = await DialogService.ShowMessageBoxAsync(
            "Remove student",
            $"Remove {student.DisplayName}? This will delete the student account and they will need to register again.",
            yesText: "Remove",
            cancelText: "Cancel");

        if (confirmed != true)
        {
            return;
        }

        if (!await ConfirmDeleteAsync())
        {
            return;
        }

        await ExecuteSchoolClassActionAsync(
            () => SchoolClassService.RemoveStudentFromClassAsync(_loginState, _selectedClassId.Value, student.Id),
            _ => LoadSchoolClassesAsync(_selectedSchoolId, _selectedClassId));
    }

    private async Task ExecuteSchoolClassActionAsync(
        Func<Task<Result<int?>>> action,
        Func<int?, Task> refresh)
    {
        _isSaving = true;
        await LoadLoginStateAsync();

        var result = await action();
        _isSaving = false;

        if (!result.Succeeded)
        {
            Snackbar.Add(result.Message, Severity.Error);
            return;
        }

        Snackbar.Add(result.Message, Severity.Success);
        await refresh(result.Value);
    }

    private async Task<bool> ConfirmDeleteAsync()
    {
        var confirmed = await DialogService.ShowMessageBoxAsync(
            "Confirmation",
            "Are you sure?",
            yesText: "Yes",
            cancelText: "Cancel");

        return confirmed == true;
    }

    private async Task<SchoolDialogInput?> ShowSchoolDialogAsync(
        string title,
        string name,
        string saveText)
    {
        var parameters = new DialogParameters<SchoolEditDialog>
        {
            { dialog => dialog.Name, name },
            { dialog => dialog.SaveText, saveText }
        };

        var dialog = await DialogService.ShowAsync<SchoolEditDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;

        return result is null || result.Canceled
            ? null
            : result.Data as SchoolDialogInput;
    }

    private async Task<ClassDialogInput?> ShowClassDialogAsync(
        string title,
        string name,
        string code,
        string saveText)
    {
        var parameters = new DialogParameters<ClassEditDialog>
        {
            { dialog => dialog.Name, name },
            { dialog => dialog.Code, code },
            { dialog => dialog.SaveText, saveText }
        };

        var dialog = await DialogService.ShowAsync<ClassEditDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;

        return result is null || result.Canceled
            ? null
            : result.Data as ClassDialogInput;
    }
}
