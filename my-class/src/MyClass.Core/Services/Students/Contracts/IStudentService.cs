
using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IStudentService
{
    Task<StudentListResult> GetStudentsForClassAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string? searchText = null,
        bool activeOnly = false,
        CancellationToken cancellationToken = default);

    Task<StudentActionResult> RemoveStudentFromClassAsync(
        LoginState? loginState,
        ClassContext currentClass,
        int studentId,
        CancellationToken cancellationToken = default);

    Task<StudentActionResult> ResetStudentsActiveStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);
}


