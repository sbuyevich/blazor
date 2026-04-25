using MyClass.Services.Auth;
using ClassContextModel = MyClass.Services.ClassContext.ClassContext;

namespace MyClass.Services.Students;

public interface IStudentService
{
    Task<StudentListResult> GetStudentsForClassAsync(
        LoginState? loginState,
        ClassContextModel currentClass,
        CancellationToken cancellationToken = default);
}
