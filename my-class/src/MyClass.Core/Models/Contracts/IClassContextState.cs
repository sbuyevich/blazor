
namespace  MyClass.Core.Models;

public interface IClassContextState
{
    ClassContext? CurrentClass { get; }

    Result<ClassContext>? Result { get; }

    event Action? Changed;

    void Set(Result<ClassContext>? result);
}
