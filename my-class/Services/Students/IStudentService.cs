using MyClass.Services.Auth;
using ClassContextModel = MyClass.Services.ClassContext.ClassContext;

namespace MyClass.Services.Students;

public interface IStudentService
{
    Task<StudentListResult> GetStudentsForClassAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        string? searchText = null,
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
