using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface ISchoolClassService
{
    Task<SchoolClassListResult> GetSchoolClassesAsync(
        LoginState? loginState,
        CancellationToken cancellationToken = default);

    Task<SchoolClassClassListResult> GetClassesForSchoolAsync(
        LoginState? loginState,
        int schoolId,
        CancellationToken cancellationToken = default);

    Task<SchoolClassActionResult> CreateSchoolAsync(
        LoginState? loginState,
        string name,
        CancellationToken cancellationToken = default);

    Task<SchoolClassActionResult> UpdateSchoolAsync(
        LoginState? loginState,
        int schoolId,
        string name,
        CancellationToken cancellationToken = default);

    Task<SchoolClassActionResult> DeleteSchoolAsync(
        LoginState? loginState,
        int schoolId,
        CancellationToken cancellationToken = default);

    Task<SchoolClassActionResult> CreateClassAsync(
        LoginState? loginState,
        int schoolId,
        string name,
        string code,
        CancellationToken cancellationToken = default);

    Task<SchoolClassActionResult> UpdateClassAsync(
        LoginState? loginState,
        int classId,
        string name,
        string code,
        CancellationToken cancellationToken = default);

    Task<SchoolClassActionResult> DeleteClassAsync(
        LoginState? loginState,
        int classId,
        CancellationToken cancellationToken = default);

    Task<SchoolClassStudentListResult> GetStudentsForClassAsync(
        LoginState? loginState,
        int classId,
        string? searchText = null,
        CancellationToken cancellationToken = default);

    Task<SchoolClassActionResult> RemoveStudentFromClassAsync(
        LoginState? loginState,
        int classId,
        int studentId,
        CancellationToken cancellationToken = default);
}
