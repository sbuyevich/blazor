
using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IStudentService
{
    Task<Result<IReadOnlyList<StudentListItem>>> GetStudentsForClassAsync(
        LoginState? loginState,
        ClassContext currentClass,
        string? searchText = null,
        bool activeOnly = false,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> RemoveStudentFromClassAsync(
        LoginState? loginState,
        ClassContext currentClass,
        int studentId,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> ResetStudentsActiveStateAsync(
        LoginState? loginState,
        ClassContext currentClass,
        CancellationToken cancellationToken = default);
}


