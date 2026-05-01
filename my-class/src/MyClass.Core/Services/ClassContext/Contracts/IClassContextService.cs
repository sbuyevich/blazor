using MyClass.Core.Models;

namespace  MyClass.Core.Services;

public interface IClassContextService
{
    Task<Result<ClassContext>> ResolveAsync(string? classCode, CancellationToken cancellationToken = default);

    string? GetClassCodeFromUri(string uri);

    string GetPathWithClassCode(string path, string? classCode);
}


