using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using student_projects.Auth;
using student_projects.Data;
using student_projects.Models;

namespace student_projects.Services;

public sealed class AuthService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IPasswordHasher<AppUser> passwordHasher) : IAuthService
{
    public async Task<AppUser?> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var trimmedUserName = userName.Trim();

        if (string.IsNullOrWhiteSpace(trimmedUserName) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.UserName == trimmedUserName, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return verificationResult == PasswordVerificationResult.Failed ? null : user;
    }

    public ClaimsPrincipal CreatePrincipal(AppUser user)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        ],
        AuthenticationConstants.Scheme);

        return new ClaimsPrincipal(identity);
    }
}
