using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using student_projects.Auth;
using student_projects.Data;
using student_projects.Models;

namespace student_projects.Services;

public sealed class AuthService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IPasswordHasher<Student> passwordHasher) : IAuthService
{
    public async Task<Student?> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var trimmedUserName = userName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedUserName) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var student = await dbContext.Students
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.UserName == trimmedUserName, cancellationToken);

        if (student is null)
        {
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(student, student.PasswordHash, password);
        return verificationResult == PasswordVerificationResult.Failed ? null : student;
    }

    public async Task<RegistrationResult> RegisterUserAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var trimmedUserName = userName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedUserName) || string.IsNullOrWhiteSpace(password))
        {
            return new RegistrationResult(false, ErrorMessage: "Username and password are required.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var duplicateExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(candidate => candidate.UserName == trimmedUserName, cancellationToken);

        if (duplicateExists)
        {
            return new RegistrationResult(false, ErrorMessage: "That username is already taken.");
        }

        var student = new Student
        {
            UserName = trimmedUserName
        };

        student.PasswordHash = passwordHasher.HashPassword(student, password);

        dbContext.Students.Add(student);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return new RegistrationResult(false, ErrorMessage: "That username is already taken.");
        }

        return new RegistrationResult(true, Student: student);
    }

    public ClaimsPrincipal CreatePrincipal(Student student)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
            new Claim(ClaimTypes.Name, student.UserName)
        ],
        AuthenticationConstants.Scheme);

        return new ClaimsPrincipal(identity);
    }
}
