namespace MyClass.Core.Services;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(
        string userName,
        string password,
        bool isTeacher,
        string classCode,
        CancellationToken cancellationToken = default);

    Task<LoginResult> RegisterStudentAsync(
        string userName,
        string firstName,
        string lastName,
        string password,
        string classCode,
        CancellationToken cancellationToken = default);
}


