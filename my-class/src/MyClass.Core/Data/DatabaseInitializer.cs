using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyClass.Core.Data.Entities;

namespace MyClass.Core.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var dbContext = await services
            .GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
            .CreateDbContextAsync();

        await dbContext.Database.EnsureCreatedAsync();
        await EnsureStudentCompatibilityColumnsAsync(dbContext);
        await EnsureQuizTablesAsync(dbContext);

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
                    Code = "DEMO",
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

    private static async Task EnsureQuizTablesAsync(ApplicationDbContext dbContext)
    {
        await DropLegacyQuizTablesAsync(dbContext);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS QuizAnswers (
                Id INTEGER NOT NULL CONSTRAINT PK_QuizAnswers PRIMARY KEY AUTOINCREMENT,
                StudentId INTEGER NOT NULL,
                StudentUserName TEXT NOT NULL,
                StudentFirstName TEXT NOT NULL,
                StudentLastName TEXT NOT NULL,
                StudentDisplayName TEXT NOT NULL,
                QuestionKey TEXT NOT NULL,
                QuestionIndex INTEGER NOT NULL,
                QuestionText TEXT NOT NULL,
                CorrectAnswer TEXT NOT NULL,
                Answer TEXT NOT NULL DEFAULT '',
                StartedAtUtc TEXT NOT NULL,
                EndedAtUtc TEXT NULL,
                AnswerRevealedAtUtc TEXT NULL,
                IsCorrect INTEGER NOT NULL DEFAULT 0,
                CONSTRAINT FK_QuizAnswers_Students_StudentId FOREIGN KEY (StudentId) REFERENCES Students (Id) ON DELETE CASCADE
            )
            """);

        await EnsureQuizAnswerCompatibilityColumnsAsync(dbContext);

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE INDEX IF NOT EXISTS IX_QuizAnswers_QuestionIndex ON QuizAnswers (QuestionIndex)");

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE INDEX IF NOT EXISTS IX_QuizAnswers_QuestionKey ON QuizAnswers (QuestionKey)");

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE INDEX IF NOT EXISTS IX_QuizAnswers_StudentId ON QuizAnswers (StudentId)");

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE INDEX IF NOT EXISTS IX_QuizAnswers_QuestionIndex_StudentId ON QuizAnswers (QuestionIndex, StudentId)");

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE INDEX IF NOT EXISTS IX_QuizAnswers_QuestionKey_StudentId ON QuizAnswers (QuestionKey, StudentId)");
    }

    private static async Task DropLegacyQuizTablesAsync(ApplicationDbContext dbContext)
    {
        var quizAnswerColumns = await dbContext.Database
            .SqlQueryRaw<string>("SELECT name AS Value FROM pragma_table_info('QuizAnswers')")
            .ToListAsync();

        if (quizAnswerColumns.Contains("QuizSessionQuestionId", StringComparer.OrdinalIgnoreCase))
        {
            await dbContext.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS QuizAnswers");
        }

        await dbContext.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS QuizSessionQuestions");
        await dbContext.Database.ExecuteSqlRawAsync("DROP TABLE IF EXISTS QuizSessions");
    }

    private static async Task EnsureQuizAnswerCompatibilityColumnsAsync(ApplicationDbContext dbContext)
    {
        var quizAnswerColumns = await dbContext.Database
            .SqlQueryRaw<string>("SELECT name AS Value FROM pragma_table_info('QuizAnswers')")
            .ToListAsync();

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "StudentUserName",
            "ALTER TABLE QuizAnswers ADD COLUMN StudentUserName TEXT NOT NULL DEFAULT ''");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "StudentFirstName",
            "ALTER TABLE QuizAnswers ADD COLUMN StudentFirstName TEXT NOT NULL DEFAULT ''");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "StudentLastName",
            "ALTER TABLE QuizAnswers ADD COLUMN StudentLastName TEXT NOT NULL DEFAULT ''");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "StudentDisplayName",
            "ALTER TABLE QuizAnswers ADD COLUMN StudentDisplayName TEXT NOT NULL DEFAULT ''");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "QuestionKey",
            "ALTER TABLE QuizAnswers ADD COLUMN QuestionKey TEXT NOT NULL DEFAULT ''");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "QuestionIndex",
            "ALTER TABLE QuizAnswers ADD COLUMN QuestionIndex INTEGER NOT NULL DEFAULT 0");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "QuestionText",
            "ALTER TABLE QuizAnswers ADD COLUMN QuestionText TEXT NOT NULL DEFAULT ''");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "CorrectAnswer",
            "ALTER TABLE QuizAnswers ADD COLUMN CorrectAnswer TEXT NOT NULL DEFAULT ''");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "Answer",
            "ALTER TABLE QuizAnswers ADD COLUMN Answer TEXT NOT NULL DEFAULT ''");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "StartedAtUtc",
            "ALTER TABLE QuizAnswers ADD COLUMN StartedAtUtc TEXT NOT NULL DEFAULT '0001-01-01T00:00:00.0000000Z'");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "EndedAtUtc",
            "ALTER TABLE QuizAnswers ADD COLUMN EndedAtUtc TEXT NULL");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "AnswerRevealedAtUtc",
            "ALTER TABLE QuizAnswers ADD COLUMN AnswerRevealedAtUtc TEXT NULL");

        await AddQuizAnswerColumnIfMissingAsync(
            dbContext,
            quizAnswerColumns,
            "IsCorrect",
            "ALTER TABLE QuizAnswers ADD COLUMN IsCorrect INTEGER NOT NULL DEFAULT 0");
    }

    private static async Task AddQuizAnswerColumnIfMissingAsync(
        ApplicationDbContext dbContext,
        IReadOnlyCollection<string> columns,
        string columnName,
        string sql)
    {
        if (columns.Contains(columnName, StringComparer.OrdinalIgnoreCase))
        {
            return;
        }

        await dbContext.Database.ExecuteSqlRawAsync(sql);
    }
}


