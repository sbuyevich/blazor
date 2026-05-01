using Microsoft.AspNetCore.Components;
using MyClass.Core.Models;

namespace  MyClass.Web.Pages;

public partial class Students
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;

    private Result<IReadOnlyList<StudentListItem>>? _studentsResult;

    private int StudentCount => _studentsResult?.Value?.Count ?? 0;

    private void OnStudentsLoaded(Result<IReadOnlyList<StudentListItem>> result)
    {
        _studentsResult = result;
    }
}
