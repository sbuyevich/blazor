using Microsoft.EntityFrameworkCore;

namespace student_projects.Data;

public static class ApplicationDbContextInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await EnsureRequestedSchemaAsync(dbContext, cancellationToken);
    }

    private static async Task EnsureRequestedSchemaAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS "School" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_School" PRIMARY KEY AUTOINCREMENT,
                "Name" TEXT NOT NULL
            );
            """,
            cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS "Class" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_Class" PRIMARY KEY AUTOINCREMENT,
                "Name" TEXT NOT NULL,
                "ShoolId" INTEGER NOT NULL,
                CONSTRAINT "FK_Class_School_ShoolId" FOREIGN KEY ("ShoolId") REFERENCES "School" ("Id") ON DELETE CASCADE
            );
            """,
            cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS "Student" (
                "Id" INTEGER NOT NULL CONSTRAINT "PK_Student" PRIMARY KEY AUTOINCREMENT,
                "ClassId" INTEGER NULL,
                CONSTRAINT "FK_Student_Class_ClassId" FOREIGN KEY ("ClassId") REFERENCES "Class" ("Id") ON DELETE SET NULL
            );
            """,
            cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS "IX_Class_ShoolId" ON "Class" ("ShoolId");
            """,
            cancellationToken);

        if (!await ColumnExistsAsync(dbContext, "Student", "ClassId", cancellationToken))
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                ALTER TABLE "Student" ADD COLUMN "ClassId" INTEGER NULL;
                """,
                cancellationToken);
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS "IX_Student_ClassId" ON "Student" ("ClassId");
            """,
            cancellationToken);
    }

    private static async Task<bool> ColumnExistsAsync(
        ApplicationDbContext dbContext,
        string tableName,
        string columnName,
        CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await dbContext.Database.OpenConnectionAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(\"{EscapeSqliteIdentifier(tableName)}\");";

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await dbContext.Database.CloseConnectionAsync();
            }
        }
    }

    private static string EscapeSqliteIdentifier(string value)
    {
        return value.Replace("\"", "\"\"");
    }
}
