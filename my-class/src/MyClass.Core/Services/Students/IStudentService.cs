using MyClass.Core.Services.Auth;
using ClassContextModel = MyClass.Core.Services.ClassContext.ClassContext;

namespace MyClass.Core.Services.Students;

public interface IStudentService
{
    Task<StudentListResult> GetStudentsForClassAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        string? searchText = null,
        bool activeOnly = false,
        CancellationToken cancellationToken = default);

    Task<StudentActionResult> RemoveStudentFromClassAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        int studentId,
        CancellationToken cancellationToken = default);

    Task<StudentActionResult> ResetStudentsActiveStateAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);
}


