using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MyClass.Core.Data;
using MyClass.Core.Models;

namespace  MyClass.Core.Services;

public sealed class ClassContextService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IClassContextService
{
    private const string ClassCodeQueryParameter = "c";
    private const string MissingCodeMessage = "Class code is missing. Add ?c=demo to the URL to load the demo class.";
    private const string NotFoundMessageTemplate = "Class code '{0}' was not found.";

    public async Task<ClassContextResult> ResolveAsync(string? classCode, CancellationToken cancellationToken = default)
    {
        var normalizedCode = classCode?.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            return new ClassContextResult(
                ClassContextStatus.MissingCode,
                null,
                MissingCodeMessage);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var currentClass = await dbContext.Classes
            .AsNoTracking()
            .Include(c => c.School)
            .Where(c => c.Code == normalizedCode)
            .Select(c => new ClassContext(
                c.SchoolId,
                c.School.Name,
                c.Id,
                c.Name,
                c.Code))
            .SingleOrDefaultAsync(cancellationToken);

        if (currentClass is null)
        {
            return new ClassContextResult(
                ClassContextStatus.NotFound,
                null,
                string.Format(NotFoundMessageTemplate, normalizedCode));
        }

        return new ClassContextResult(
            ClassContextStatus.Loaded,
            currentClass,
            string.Empty);
    }

    public string? GetClassCodeFromUri(string uri)
    {
        var query = QueryHelpers.ParseQuery(new Uri(uri).Query);

        return query.TryGetValue(ClassCodeQueryParameter, out var classCode)
            ? classCode.FirstOrDefault()
            : null;
    }

    public string GetPathWithClassCode(string path, string? classCode)
    {
        var normalizedPath = string.IsNullOrWhiteSpace(path) ? "/" : path;

        if (!normalizedPath.StartsWith('/'))
        {
            normalizedPath = $"/{normalizedPath}";
        }

        return string.IsNullOrWhiteSpace(classCode)
            ? normalizedPath
            : QueryHelpers.AddQueryString(normalizedPath, ClassCodeQueryParameter, classCode.Trim());
    }
}


