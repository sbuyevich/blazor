using Microsoft.EntityFrameworkCore;
using MyClass.Data.Entities;

namespace MyClass.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<School> Schools => Set<School>();

    public DbSet<Class> Classes => Set<Class>();

    public DbSet<Student> Students => Set<Student>();

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
    }
}
