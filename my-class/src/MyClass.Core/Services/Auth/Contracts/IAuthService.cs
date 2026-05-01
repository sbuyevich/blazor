using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IAuthService
{
    Task<Result<LoginState>> LoginAsync(
        string userName,
        string password,
        bool isTeacher,
        string classCode,
        CancellationToken cancellationToken = default);

    Task<Result<LoginState>> RegisterStudentAsync(
        string userName,
        string firstName,
        string lastName,
        string password,
        string classCode,
        CancellationToken cancellationToken = default);
}


