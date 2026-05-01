namespace  MyClass.Core.Models;

public sealed class ClassContextState : IClassContextState
{
    public ClassContext? CurrentClass { get; private set; }

    public Result<ClassContext>? Result { get; private set; }

    public event Action? Changed;

    public void Set(Result<ClassContext>? result)
    {
        Result = result;
        CurrentClass = result?.Value;
        Changed?.Invoke();
    }
}
