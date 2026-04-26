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

    private static async Task EnsureQuizTablesAsync(ApplicationDbContext dbContext)
    {
        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS QuizSessions (
                Id INTEGER NOT NULL CONSTRAINT PK_QuizSessions PRIMARY KEY AUTOINCREMENT,
                ClassId INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Status INTEGER NOT NULL,
                ActiveQuestionIndex INTEGER NOT NULL,
                StartedAtUtc TEXT NOT NULL,
                CompletedAtUtc TEXT NULL,
                CONSTRAINT FK_QuizSessions_Classes_ClassId FOREIGN KEY (ClassId) REFERENCES Classes (Id) ON DELETE CASCADE
            )
            """);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS QuizSessionQuestions (
                Id INTEGER NOT NULL CONSTRAINT PK_QuizSessionQuestions PRIMARY KEY AUTOINCREMENT,
                QuizSessionId INTEGER NOT NULL,
                QuestionIndex INTEGER NOT NULL,
                QuestionKey TEXT NOT NULL,
                Title TEXT NOT NULL,
                TimeoutSeconds INTEGER NOT NULL,
                CorrectAnswer INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                StartedAtUtc TEXT NOT NULL,
                FinishedAtUtc TEXT NULL,
                CONSTRAINT FK_QuizSessionQuestions_QuizSessions_QuizSessionId FOREIGN KEY (QuizSessionId) REFERENCES QuizSessions (Id) ON DELETE CASCADE
            )
            """);

        await dbContext.Database.ExecuteSqlRawAsync(
            """
            CREATE TABLE IF NOT EXISTS QuizAnswers (
                Id INTEGER NOT NULL CONSTRAINT PK_QuizAnswers PRIMARY KEY AUTOINCREMENT,
                QuizSessionQuestionId INTEGER NOT NULL,
                StudentId INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                SelectedAnswer INTEGER NULL,
                IsCorrect INTEGER NULL,
                CreatedAtUtc TEXT NOT NULL,
                SubmittedAtUtc TEXT NULL,
                CONSTRAINT FK_QuizAnswers_QuizSessionQuestions_QuizSessionQuestionId FOREIGN KEY (QuizSessionQuestionId) REFERENCES QuizSessionQuestions (Id) ON DELETE CASCADE,
                CONSTRAINT FK_QuizAnswers_Students_StudentId FOREIGN KEY (StudentId) REFERENCES Students (Id) ON DELETE CASCADE
            )
            """);

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE INDEX IF NOT EXISTS IX_QuizSessions_ClassId_Status ON QuizSessions (ClassId, Status)");

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE UNIQUE INDEX IF NOT EXISTS IX_QuizSessionQuestions_QuizSessionId_QuestionIndex ON QuizSessionQuestions (QuizSessionId, QuestionIndex)");

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE UNIQUE INDEX IF NOT EXISTS IX_QuizAnswers_QuizSessionQuestionId_StudentId ON QuizAnswers (QuizSessionQuestionId, StudentId)");

        await dbContext.Database.ExecuteSqlRawAsync(
            "CREATE INDEX IF NOT EXISTS IX_QuizAnswers_StudentId ON QuizAnswers (StudentId)");
    }
}
