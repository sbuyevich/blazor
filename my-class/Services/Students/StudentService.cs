using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Data;
using MyClass.Options;
using MyClass.Services.Auth;
using ClassContextModel = MyClass.Services.ClassContext.ClassContext;

namespace MyClass.Services.Students;

public sealed class StudentService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IOptions<Teacher> teacherOptions) : IStudentService
{
    public async Task<StudentListResult> GetStudentsForClassAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default)
    {
        if (loginState is null)
        {
            return StudentListResult.Failure("Sign in as the teacher to view students.");
        }

        if (!loginState.IsTeacher)
        {
            return StudentListResult.Failure("Only teachers can view the student list.");
        }

        if (!string.Equals(loginState.ClassCode, currentClass.Code, StringComparison.OrdinalIgnoreCase))
        {
            return StudentListResult.Failure("Sign in as the teacher for this class to view students.");
        }

        var teacher = teacherOptions.Value;

        if (!string.Equals(loginState.UserName, teacher.UserName, StringComparison.OrdinalIgnoreCase))
        {
            return StudentListResult.Failure("Teacher login is required to view students.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var students = await dbContext.Students
            .AsNoTracking()
            .Where(student => student.ClassId == currentClass.ClassId)
            .OrderBy(student => student.LastName)
            .ThenBy(student => student.FirstName)
            .ThenBy(student => student.UserName)
            .Select(student => new StudentListItem(
                student.Id,
                student.UserName,
                student.FirstName,
                student.LastName,
                student.DisplayName,
                student.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return StudentListResult.Success(students);
    }
}
