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
}
