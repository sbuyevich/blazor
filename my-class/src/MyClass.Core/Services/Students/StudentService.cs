using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Core.Data; 
using MyClass.Core.Options;
using MyClass.Core.Services.Auth;
using ClassContextModel = MyClass.Core.Services.ClassContext.ClassContext;

namespace MyClass.Core.Services.Students;   

public sealed class StudentService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IOptions<TeacherOptions> teacherOptions) : IStudentService
{
    public async Task<StudentListResult> GetStudentsForClassAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        string? searchText = null,
        bool activeOnly = false,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState, currentClass);

        if (authorizationMessage is not null)
        {
            return StudentListResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var query = dbContext.Students
            .AsNoTracking()
            .Where(student => student.ClassId == currentClass.ClassId);

        var normalizedSearchText = searchText?.Trim();

        if (!string.IsNullOrWhiteSpace(normalizedSearchText))
        {
            query = query.Where(student =>
                student.FirstName.Contains(normalizedSearchText) ||
                student.LastName.Contains(normalizedSearchText) ||
                student.DisplayName.Contains(normalizedSearchText));
        }

        if (activeOnly)
        {
            query = query.Where(student => student.IsActive);
        }

        var students = await query
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ThenBy(student => student.UserName)
            .Select(student => new StudentListItem(
                student.Id,
                student.UserName,
                student.FirstName,
                student.LastName,
                student.DisplayName,
                student.IsActive,
                student.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return StudentListResult.Success(students);
    }

    public async Task<StudentActionResult> RemoveStudentFromClassAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        int studentId,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState, currentClass);

        if (authorizationMessage is not null)
        {
            return StudentActionResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var student = await dbContext.Students
            .SingleOrDefaultAsync(
                student =>
                    student.Id == studentId &&
                    student.ClassId == currentClass.ClassId,
                cancellationToken);

        if (student is null)
        {
            return StudentActionResult.Failure("The selected student was not found in this class.");
        }

        dbContext.Students.Remove(student);
        await dbContext.SaveChangesAsync(cancellationToken);

        return StudentActionResult.Success($"{student.DisplayName} was removed.");
    }

    public async Task<StudentActionResult> ResetStudentsActiveStateAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default)
    {
        var authorizationMessage = ValidateTeacherAccess(loginState, currentClass);

        if (authorizationMessage is not null)
        {
            return StudentActionResult.Failure(authorizationMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var updatedCount = await dbContext.Students
            .Where(student => student.ClassId == currentClass.ClassId && student.IsActive)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(student => student.IsActive, false),
                cancellationToken);

        return StudentActionResult.Success($"Reset {updatedCount} active student{(updatedCount == 1 ? string.Empty : "s")}.");
    }

    private string? ValidateTeacherAccess(LoginState? loginState, ClassContextModel currentClass)
    {
        if (loginState is null)
        {
            return "Sign in as the teacher to view students.";
        }

        if (!loginState.IsTeacher)
        {
            return "Only teachers can manage the student list.";
        }

        if (!string.Equals(loginState.ClassCode, currentClass.Code, StringComparison.OrdinalIgnoreCase))
        {
            return "Sign in as the teacher for this class to manage students.";
        }

        var teacher = teacherOptions.Value;

        return string.Equals(loginState.UserName, teacher.UserName, StringComparison.OrdinalIgnoreCase)
            ? null
            : "Teacher login is required to manage students.";
    }
}


