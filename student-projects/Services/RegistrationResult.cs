using student_projects.Models;

namespace student_projects.Services;

public sealed record RegistrationResult(bool Succeeded, AppUser? User = null, string? ErrorMessage = null);
