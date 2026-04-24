using Microsoft.EntityFrameworkCore;

namespace student_projects.Data;

public static class ApplicationDbContextInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        var seedOptions = scope.ServiceProvider.GetRequiredService<DatabaseSeedOptions>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await EnsureRequestedSchemaAsync(dbContext, cancellationToken);
        await ApplicationDbContextSeed.SeedAsync(dbContext, seedOptions, cancellationToken);
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
                "UserName" TEXT NOT NULL,
                "PasswordHash" TEXT NOT NULL,
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

        if (!await ColumnExistsAsync(dbContext, "Student", "UserName", cancellationToken))
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                ALTER TABLE "Student" ADD COLUMN "UserName" TEXT NULL;
                """,
                cancellationToken);
        }

        if (!await ColumnExistsAsync(dbContext, "Student", "PasswordHash", cancellationToken))
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                ALTER TABLE "Student" ADD COLUMN "PasswordHash" TEXT NULL;
                """,
                cancellationToken);
        }

        if (!await ColumnExistsAsync(dbContext, "Student", "ClassId", cancellationToken))
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                ALTER TABLE "Student" ADD COLUMN "ClassId" INTEGER NULL;
                """,
                cancellationToken);
        }

        await MigrateLegacyUsersAsync(dbContext, cancellationToken);

        var duplicateStudentUserNames = await ExecuteLongScalarAsync(
            dbContext,
            """
            SELECT COUNT(*)
            FROM (
                SELECT "UserName"
                FROM "Student"
                WHERE "UserName" IS NOT NULL AND "UserName" <> ''
                GROUP BY "UserName"
                HAVING COUNT(*) > 1
            );
            """,
            cancellationToken);

        if (duplicateStudentUserNames > 0)
        {
            throw new InvalidOperationException("Cannot use Student for login because duplicate Student.UserName values exist.");
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_Student_UserName" ON "Student" ("UserName");
            """,
            cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS "IX_Student_ClassId" ON "Student" ("ClassId");
            """,
            cancellationToken);

        await EnsureStudentProjectOwnershipSchemaAsync(dbContext, cancellationToken);
    }

    private static async Task MigrateLegacyUsersAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        if (!await TableExistsAsync(dbContext, "Users", cancellationToken))
        {
            return;
        }

        var conflictingStudentRows = await ExecuteLongScalarAsync(
            dbContext,
            """
            SELECT COUNT(*)
            FROM "Users" AS "u"
            INNER JOIN "Student" AS "s" ON "s"."Id" = "u"."Id"
            WHERE ("s"."UserName" IS NOT NULL AND "s"."UserName" <> '' AND "s"."UserName" <> "u"."UserName")
               OR ("s"."PasswordHash" IS NOT NULL AND "s"."PasswordHash" <> '' AND "s"."PasswordHash" <> "u"."PasswordHash");
            """,
            cancellationToken);

        if (conflictingStudentRows > 0)
        {
            throw new InvalidOperationException("Cannot migrate Users to Student because a Student row with the same Id already has different credentials.");
        }

        var conflictingUserNames = await ExecuteLongScalarAsync(
            dbContext,
            """
            SELECT COUNT(*)
            FROM "Users" AS "u"
            INNER JOIN "Student" AS "s" ON "s"."UserName" = "u"."UserName" AND "s"."Id" <> "u"."Id";
            """,
            cancellationToken);

        if (conflictingUserNames > 0)
        {
            throw new InvalidOperationException("Cannot migrate Users to Student because a legacy username already belongs to a different Student.");
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            UPDATE "Student"
            SET "UserName" = CASE
                    WHEN "UserName" IS NULL OR "UserName" = ''
                    THEN (SELECT "UserName" FROM "Users" WHERE "Users"."Id" = "Student"."Id")
                    ELSE "UserName"
                END,
                "PasswordHash" = CASE
                    WHEN "PasswordHash" IS NULL OR "PasswordHash" = ''
                    THEN (SELECT "PasswordHash" FROM "Users" WHERE "Users"."Id" = "Student"."Id")
                    ELSE "PasswordHash"
                END
            WHERE EXISTS (
                SELECT 1
                FROM "Users"
                WHERE "Users"."Id" = "Student"."Id"
            );
            """,
            cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            INSERT INTO "Student" ("Id", "UserName", "PasswordHash", "ClassId")
            SELECT "u"."Id", "u"."UserName", "u"."PasswordHash", NULL
            FROM "Users" AS "u"
            WHERE NOT EXISTS (
                SELECT 1
                FROM "Student" AS "s"
                WHERE "s"."Id" = "u"."Id"
            );
            """,
            cancellationToken);
    }

    private static async Task EnsureStudentProjectOwnershipSchemaAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        if (!await TableExistsAsync(dbContext, "StudentProjects", cancellationToken))
        {
            return;
        }

        if (!await ColumnExistsAsync(dbContext, "StudentProjects", "OwnerStudentId", cancellationToken))
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                ALTER TABLE "StudentProjects" ADD COLUMN "OwnerStudentId" INTEGER NULL;
                """,
                cancellationToken);
        }

        if (await ColumnExistsAsync(dbContext, "StudentProjects", "OwnerUserId", cancellationToken))
        {
            var conflictingOwnershipRows = await ExecuteLongScalarAsync(
                dbContext,
                """
                SELECT COUNT(*)
                FROM "StudentProjects"
                WHERE "OwnerStudentId" IS NOT NULL
                  AND "OwnerUserId" IS NOT NULL
                  AND "OwnerStudentId" <> "OwnerUserId";
                """,
                cancellationToken);

            if (conflictingOwnershipRows > 0)
            {
                throw new InvalidOperationException("Cannot migrate StudentProjects ownership because OwnerStudentId conflicts with legacy OwnerUserId.");
            }

            await dbContext.Database.ExecuteSqlRawAsync(
                """
                UPDATE "StudentProjects"
                SET "OwnerStudentId" = "OwnerUserId"
                WHERE "OwnerStudentId" IS NULL AND "OwnerUserId" IS NOT NULL;
                """,
                cancellationToken);
        }

        var unmappedOwnershipRows = await ExecuteLongScalarAsync(
            dbContext,
            """
            SELECT COUNT(*)
            FROM "StudentProjects"
            WHERE "OwnerStudentId" IS NULL;
            """,
            cancellationToken);

        if (unmappedOwnershipRows > 0)
        {
            throw new InvalidOperationException("Cannot use Student for project ownership because some StudentProjects rows do not have an OwnerStudentId.");
        }

        var missingOwnerRows = await ExecuteLongScalarAsync(
            dbContext,
            """
            SELECT COUNT(*)
            FROM "StudentProjects" AS "p"
            LEFT JOIN "Student" AS "s" ON "s"."Id" = "p"."OwnerStudentId"
            WHERE "s"."Id" IS NULL;
            """,
            cancellationToken);

        if (missingOwnerRows > 0)
        {
            throw new InvalidOperationException("Cannot use Student for project ownership because some StudentProjects owners do not exist in Student.");
        }

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE INDEX IF NOT EXISTS "IX_StudentProjects_OwnerStudentId" ON "StudentProjects" ("OwnerStudentId");
            """,
            cancellationToken);
    }

    private static async Task<bool> TableExistsAsync(
        ApplicationDbContext dbContext,
        string tableName,
        CancellationToken cancellationToken)
    {
        var escapedTableName = EscapeSqliteString(tableName);
        var tableCount = await ExecuteLongScalarAsync(
            dbContext,
            $"""
            SELECT COUNT(*)
            FROM "sqlite_master"
            WHERE "type" = 'table' AND "name" = '{escapedTableName}';
            """,
            cancellationToken);

        return tableCount > 0;
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

    private static string EscapeSqliteString(string value)
    {
        return value.Replace("'", "''");
    }

    private static async Task<long> ExecuteLongScalarAsync(
        ApplicationDbContext dbContext,
        string commandText,
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
            command.CommandText = commandText;
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt64(result);
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await dbContext.Database.CloseConnectionAsync();
            }
        }
    }
}
