namespace MyClass.Services.Auth;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(
        string userName,
        string password,
        bool isTeacher,
        string classCode,
        CancellationToken cancellationToken = default);
}
