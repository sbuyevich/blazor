using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface ISchoolClassService
{
    Task<Result<IReadOnlyList<SchoolClassSchoolItem>>> GetSchoolClassesAsync(
        LoginState? loginState,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<SchoolClassClassItem>>> GetClassesForSchoolAsync(
        LoginState? loginState,
        int schoolId,
        CancellationToken cancellationToken = default);

    Task<Result<int?>> CreateSchoolAsync(
        LoginState? loginState,
        string name,
        CancellationToken cancellationToken = default);

    Task<Result<int?>> UpdateSchoolAsync(
        LoginState? loginState,
        int schoolId,
        string name,
        CancellationToken cancellationToken = default);

    Task<Result<int?>> DeleteSchoolAsync(
        LoginState? loginState,
        int schoolId,
        CancellationToken cancellationToken = default);

    Task<Result<int?>> CreateClassAsync(
        LoginState? loginState,
        int schoolId,
        string name,
        string code,
        CancellationToken cancellationToken = default);

    Task<Result<int?>> UpdateClassAsync(
        LoginState? loginState,
        int classId,
        string name,
        string code,
        CancellationToken cancellationToken = default);

    Task<Result<int?>> DeleteClassAsync(
        LoginState? loginState,
        int classId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<SchoolClassStudentItem>>> GetStudentsForClassAsync(
        LoginState? loginState,
        int classId,
        string? searchText = null,
        CancellationToken cancellationToken = default);

    Task<Result<int?>> RemoveStudentFromClassAsync(
        LoginState? loginState,
        int classId,
        int studentId,
        CancellationToken cancellationToken = default);
}
