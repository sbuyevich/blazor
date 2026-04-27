namespace MyClass.Core.Services.ClassContext;

public interface IClassContextService
{
    Task<ClassContextResult> ResolveAsync(string? classCode, CancellationToken cancellationToken = default);

    string? GetClassCodeFromUri(string uri);

    string GetPathWithClassCode(string path, string? classCode);
}


