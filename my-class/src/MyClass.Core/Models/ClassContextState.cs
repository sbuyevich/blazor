namespace  MyClass.Core.Models;

public sealed class ClassContextState : IClassContextState
{
    public ClassContext? CurrentClass { get; private set; }

    public ClassContextResult? Result { get; private set; }

    public event Action? Changed;

    public void Set(ClassContextResult? result)
    {
        Result = result;
        CurrentClass = result?.CurrentClass;
        Changed?.Invoke();
    }
}
