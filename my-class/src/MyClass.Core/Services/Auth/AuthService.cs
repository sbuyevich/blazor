using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyClass.Core.Data;
using MyClass.Core.Data.Entities;
using MyClass.Core.Models;
using MyClass.Core.Options;

namespace MyClass.Core.Services;

public sealed class AuthService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IPasswordHashService passwordHashService,
    IOptions<TeacherOptions> teacherOptions) : IAuthService
{
    public async Task<Result<LoginState>> LoginAsync(
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
            return Result<LoginState>.Failure("Username, password, and class are required.");
        }

        var teacher = teacherOptions.Value;
        var validTeacher =
            string.Equals(normalizedUserName, teacher.UserName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(password, teacher.Password, StringComparison.Ordinal);

        if (validTeacher)
        {
            return Result<LoginState>.Success(new LoginState(normalizedUserName, true, normalizedClassCode, "Teacher"));
        }

        if (isTeacher)
        {
            return Result<LoginState>.Failure("Invalid username or password.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var student = await dbContext.Students
            .Where(student => student.Class.Code == normalizedClassCode)
            .SingleOrDefaultAsync(
                student => student.UserName. Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase),
                cancellationToken);

        if (student is null || !passwordHashService.Verify(password, student.PasswordHash))
        {
            return Result<LoginState>.Failure("Invalid username or password.");
        }

        if (!student.IsActive)
        {
            student.IsActive = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result<LoginState>.Success(new LoginState(student.UserName, false, normalizedClassCode, student.DisplayName));
    }

    public async Task<Result<LoginState>> RegisterStudentAsync(
        string userName,
        string firstName,
        string lastName,
        string password,
        string classCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = userName.Trim();
        var normalizedFirstName = firstName.Trim();
        var normalizedLastName = lastName.Trim();
        var normalizedClassCode = classCode.Trim();

        if (string.IsNullOrWhiteSpace(normalizedUserName) ||
            string.IsNullOrWhiteSpace(normalizedFirstName) ||
            string.IsNullOrWhiteSpace(normalizedLastName) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(normalizedClassCode))
        {
            return Result<LoginState>.Failure("First name, last name, username, password, and class are required.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var currentClass = await dbContext.Classes
            .SingleOrDefaultAsync(@class => @class.Code == normalizedClassCode, cancellationToken);

        if (currentClass is null)
        {
            return Result<LoginState>.Failure("A valid class is required before registration.");
        }

        var duplicateExists = await dbContext.Students
            .AnyAsync(
                student =>
                    student.ClassId == currentClass.Id &&
                    student.UserName.Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase),
                cancellationToken);

        if (duplicateExists)
        {
            return Result<LoginState>.Failure("That username is already registered for this class.");
        }

        var student = new Student
        {
            ClassId = currentClass.Id,
            UserName = normalizedUserName,
            FirstName = normalizedFirstName,
            LastName = normalizedLastName,
            DisplayName = $"{normalizedFirstName} {normalizedLastName}",
            PasswordHash = passwordHashService.Hash(password),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Students.Add(student);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Result<LoginState>.Failure("That username is already registered for this class.");
        }

        return Result<LoginState>.Success(new LoginState(student.UserName, false, normalizedClassCode, student.DisplayName));
    }
}


