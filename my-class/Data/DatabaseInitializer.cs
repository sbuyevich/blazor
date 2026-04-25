using Microsoft.EntityFrameworkCore;
using MyClass.Data.Entities;

namespace MyClass.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var dbContext = await services
            .GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
            .CreateDbContextAsync();

        await dbContext.Database.EnsureCreatedAsync();
        await EnsureStudentCompatibilityColumnsAsync(dbContext);

        if (await dbContext.Schools.AnyAsync())
        {
            return;
        }

        var school = new School
        {
            Name = "Demo School",
            Classes =
            [
                new Class
                {
                    Code = "demo",
                    Name = "Demo Class"
                }
            ]
        };

        dbContext.Schools.Add(school);
        await dbContext.SaveChangesAsync();
    }

    private static async Task EnsureStudentCompatibilityColumnsAsync(ApplicationDbContext dbContext)
    {
        var studentColumns = await dbContext.Database
            .SqlQueryRaw<string>("SELECT name AS Value FROM pragma_table_info('Students')")
            .ToListAsync();

        if (!studentColumns.Contains("FirstName", StringComparer.OrdinalIgnoreCase))
        {
            await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Students ADD COLUMN FirstName TEXT NOT NULL DEFAULT ''");
        }

        if (!studentColumns.Contains("LastName", StringComparer.OrdinalIgnoreCase))
        {
            await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Students ADD COLUMN LastName TEXT NOT NULL DEFAULT ''");
        }

        if (!studentColumns.Contains("IsActive", StringComparer.OrdinalIgnoreCase))
        {
            await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Students ADD COLUMN IsActive INTEGER NOT NULL DEFAULT 0");
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            "UPDATE Students SET FirstName = DisplayName WHERE FirstName = '' AND LastName = '' AND DisplayName <> ''");
    }
}
