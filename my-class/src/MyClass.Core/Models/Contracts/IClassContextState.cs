
namespace  MyClass.Core.Models;

public interface IClassContextState
{
    ClassContext? CurrentClass { get; }

    ClassContextResult? Result { get; }

    event Action? Changed;

    void Set(ClassContextResult? result);
}
