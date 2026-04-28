namespace  MyClass.Core.Models;

public enum ClassContextStatus
{
    Loaded,
    MissingCode,
    NotFound
}

public sealed record ClassContextResult(
    ClassContextStatus Status,
    ClassContext? CurrentClass,
    string Message)
{
    public bool IsLoaded => Status == ClassContextStatus.Loaded && CurrentClass is not null;
}


