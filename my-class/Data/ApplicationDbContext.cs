using Microsoft.EntityFrameworkCore;
using MyClass.Data.Entities;

namespace MyClass.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<School> Schools => Set<School>();

    public DbSet<Class> Classes => Set<Class>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<QuizSession> QuizSessions => Set<QuizSession>();

    public DbSet<QuizSessionQuestion> QuizSessionQuestions => Set<QuizSessionQuestion>();

    public DbSet<QuizAnswer> QuizAnswers => Set<QuizAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<School>(entity =>
        {
            entity.Property(school => school.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasMany(school => school.Classes)
                .WithOne(@class => @class.School)
                .HasForeignKey(@class => @class.SchoolId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.Property(@class => @class.Code)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(@class => @class.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasIndex(@class => @class.Code)
                .IsUnique();

            entity.HasMany(@class => @class.Students)
                .WithOne(student => student.Class)
                .HasForeignKey(student => student.ClassId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(student => student.UserName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(student => student.DisplayName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(student => student.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(student => student.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(student => student.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(student => student.IsActive)
                .HasDefaultValue(false)
                .IsRequired();

            entity.HasIndex(student => new { student.ClassId, student.UserName })
                .IsUnique();
        });

        modelBuilder.Entity<QuizSession>(entity =>
        {
            entity.Property(session => session.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(session => session.Status)
                .IsRequired();

            entity.HasIndex(session => new { session.ClassId, session.Status });

            entity.HasOne(session => session.Class)
                .WithMany()
                .HasForeignKey(session => session.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(session => session.Questions)
                .WithOne(question => question.QuizSession)
                .HasForeignKey(question => question.QuizSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuizSessionQuestion>(entity =>
        {
            entity.Property(question => question.QuestionKey)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(question => question.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(question => question.Status)
                .IsRequired();

            entity.HasIndex(question => new { question.QuizSessionId, question.QuestionIndex })
                .IsUnique();

            entity.HasMany(question => question.Answers)
                .WithOne(answer => answer.QuizSessionQuestion)
                .HasForeignKey(answer => answer.QuizSessionQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuizAnswer>(entity =>
        {
            entity.Property(answer => answer.Status)
                .IsRequired();

            entity.HasIndex(answer => new { answer.QuizSessionQuestionId, answer.StudentId })
                .IsUnique();

            entity.HasOne(answer => answer.Student)
                .WithMany()
                .HasForeignKey(answer => answer.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
