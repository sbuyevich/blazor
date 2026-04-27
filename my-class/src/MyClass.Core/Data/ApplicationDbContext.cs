using Microsoft.EntityFrameworkCore;
using MyClass.Core.Data.Entities;

namespace MyClass.Core.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<School> Schools => Set<School>();

    public DbSet<Class> Classes => Set<Class>();

    public DbSet<Student> Students => Set<Student>();

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

        modelBuilder.Entity<QuizAnswer>(entity =>
        {
            entity.Property(answer => answer.StudentUserName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(answer => answer.StudentFirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(answer => answer.StudentLastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(answer => answer.StudentDisplayName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(answer => answer.QuestionKey)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(answer => answer.QuestionText)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(answer => answer.CorrectAnswer)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(answer => answer.Answer)
                .HasMaxLength(50)
                .HasDefaultValue(string.Empty)
                .IsRequired();

            entity.Property(answer => answer.IsCorrect)
                .HasDefaultValue(false)
                .IsRequired();

            entity.HasIndex(answer => answer.QuestionIndex);

            entity.HasIndex(answer => answer.QuestionKey);

            entity.HasIndex(answer => answer.StudentId);

            entity.HasIndex(answer => new { answer.QuestionIndex, answer.StudentId });

            entity.HasIndex(answer => new { answer.QuestionKey, answer.StudentId });

            entity.HasOne(answer => answer.Student)
                .WithMany()
                .HasForeignKey(answer => answer.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}


