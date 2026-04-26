using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MyClass.Components;
using MyClass.Data;

namespace MyClass.Services.ClassContext;

public sealed class ClassContextService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IClassContextService
{
    public async Task<ClassContextResult> ResolveAsync(string? classCode, CancellationToken cancellationToken = default)
    {
        var normalizedCode = classCode?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            if (App.CurrentClass is not null)
            {
                return new ClassContextResult(
                    ClassContextStatus.Loaded,
                    App.CurrentClass,
                    string.Empty);
            }

            App.CurrentClass = null;

            return new ClassContextResult(
                ClassContextStatus.MissingCode,
                null,
                "Class code is missing. Add ?c=demo to the URL to load the demo class.");
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        var currentClass = await dbContext.Classes
            .AsNoTracking()
            .Include(@class => @class.School)
            .Where(@class => @class.Code == normalizedCode)
            .Select(@class => new ClassContext(
                @class.SchoolId,
                @class.School.Name,
                @class.Id,
                @class.Name,
                @class.Code))
            .SingleOrDefaultAsync(cancellationToken);

        if (currentClass is null)
        {
            App.CurrentClass = null;

            return new ClassContextResult(
                ClassContextStatus.NotFound,
                null,
                $"Class code '{normalizedCode}' was not found.");
        }

        App.CurrentClass = currentClass;

        return new ClassContextResult(
            ClassContextStatus.Loaded,
            currentClass,
            string.Empty);
    }

    public string? GetClassCodeFromUri(string uri)
    {
        var query = QueryHelpers.ParseQuery(new Uri(uri).Query);

        return query.TryGetValue("c", out var classCode)
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
            : QueryHelpers.AddQueryString(normalizedPath, "c", classCode.Trim());
    }
}
