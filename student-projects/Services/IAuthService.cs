using System.Security.Claims;
using student_projects.Models;

namespace student_projects.Services;

public interface IAuthService
{
    Task<ClaimsPrincipal?> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken = default);

    Task<RegistrationResult> RegisterUserAsync(string userName, string password, CancellationToken cancellationToken = default);

    ClaimsPrincipal CreatePrincipal(Student student);
}
