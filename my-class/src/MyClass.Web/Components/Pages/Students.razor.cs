using Microsoft.AspNetCore.Components;
using  MyClass.Core.Services;
using MyClass.Core.Services;

namespace MyClass.Web.Components.Pages;

public partial class Students
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;

    private StudentListResult? _studentsResult;

    private void OnStudentsLoaded(StudentListResult result)
    {
        _studentsResult = result;
    }
}