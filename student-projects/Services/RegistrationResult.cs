using student_projects.Models;

namespace student_projects.Services;

public sealed record RegistrationResult(bool Succeeded, Student? Student = null, string? ErrorMessage = null);
