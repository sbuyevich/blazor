using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Data;
using MyClass.Options;

namespace MyClass.Services.Auth;

public sealed class AuthService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IPasswordHashService passwordHashService,
    IOptions<TeacherOptions> teacherOptions) : IAuthService
{
    public async Task<LoginResult> LoginAsync(
        string userName,
        string password,
        bool isTeacher,
        string classCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userName.Trim();
        var normalizedClassCode = classCode.Trim();

        if (string.IsNullOrWhiteSpace(normalizedUserName) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(normalizedClassCode))
        {
            return LoginResult.Failure("Username, password, and class are required.");
        }

        if (isTeacher)
        {
            var teacher = teacherOptions.Value;
            var validTeacher =
                string.Equals(normalizedUserName, teacher.UserName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(password, teacher.Password, StringComparison.Ordinal);

            return validTeacher
                ? LoginResult.Success(new LoginState(normalizedUserName, true, normalizedClassCode))
                : LoginResult.Failure("Invalid username or password.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var student = await dbContext.Students
            .AsNoTracking()
            .Where(student => student.Class.Code == normalizedClassCode)
            .SingleOrDefaultAsync(
                student => student.UserName == normalizedUserName,
                cancellationToken);

        if (student is null || !passwordHashService.Verify(password, student.PasswordHash))
        {
            return LoginResult.Failure("Invalid username or password.");
        }

        return LoginResult.Success(new LoginState(student.UserName, false, normalizedClassCode));
    }
}
