using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using student_projects.Models;

namespace student_projects.Data;

public static class ApplicationDbContextSeed
{
    public static void Seed(DbContext context, DatabaseSeedOptions options)
    {
        var resolvedOptions = ResolveOptions(options);
        var users = context.Set<AppUser>();

        if (users.Any(user => user.UserName == resolvedOptions.UserName))
        {
            return;
        }

        var user = new AppUser
        {
            UserName = resolvedOptions.UserName
        };

        var passwordHasher = new PasswordHasher<AppUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, resolvedOptions.Password);

        users.Add(user);
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, DatabaseSeedOptions options, CancellationToken cancellationToken)
    {
        var resolvedOptions = ResolveOptions(options);
        var users = context.Set<AppUser>();

        if (await users.AnyAsync(user => user.UserName == resolvedOptions.UserName, cancellationToken))
        {
            return;
        }

        var user = new AppUser
        {
            UserName = resolvedOptions.UserName
        };

        var passwordHasher = new PasswordHasher<AppUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, resolvedOptions.Password);

        await users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static DatabaseSeedOptions ResolveOptions(DatabaseSeedOptions? options)
    {
        var resolvedOptions = options ?? new DatabaseSeedOptions();

        if (string.IsNullOrWhiteSpace(resolvedOptions.UserName))
        {
            resolvedOptions.UserName = DatabaseSeedOptions.DefaultUserName;
        }

        if (string.IsNullOrWhiteSpace(resolvedOptions.Password))
        {
            resolvedOptions.Password = DatabaseSeedOptions.DefaultPassword;
        }

        return resolvedOptions;
    }
}
