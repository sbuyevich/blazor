using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using student_projects.Models;

namespace student_projects.Data;

public static class ApplicationDbContextSeed
{
    public static void Seed(DbContext context, DatabaseSeedOptions options)
    {
        var resolvedOptions = ResolveOptions(options);
        var students = context.Set<Student>();

        if (students.Any(student => student.UserName == resolvedOptions.UserName))
        {
            return;
        }

        var student = new Student
        {
            UserName = resolvedOptions.UserName
        };

        var passwordHasher = new PasswordHasher<Student>();
        student.PasswordHash = passwordHasher.HashPassword(student, resolvedOptions.Password);

        students.Add(student);
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, DatabaseSeedOptions options, CancellationToken cancellationToken)
    {
        var resolvedOptions = ResolveOptions(options);
        var students = context.Set<Student>();

        if (await students.AnyAsync(student => student.UserName == resolvedOptions.UserName, cancellationToken))
        {
            return;
        }

        var student = new Student
        {
            UserName = resolvedOptions.UserName
        };

        var passwordHasher = new PasswordHasher<Student>();
        student.PasswordHash = passwordHasher.HashPassword(student, resolvedOptions.Password);

        await students.AddAsync(student, cancellationToken);
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
