using Microsoft.EntityFrameworkCore;
using student_projects.Models;

namespace student_projects.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<StudentProject> StudentProjects => Set<StudentProject>();

    public DbSet<School> Schools => Set<School>();

    public DbSet<Class> Classes => Set<Class>();

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StudentProject>(entity =>
        {
            entity.ToTable("StudentProjects");
            entity.HasKey(project => project.Id);
            entity.Property(project => project.Name)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(project => project.Description)
                .HasMaxLength(4000)
                .IsRequired();
            entity.Property(project => project.CreatedAtUtc)
                .IsRequired();
            entity.HasOne(project => project.Owner)
                .WithMany(student => student.OwnedProjects)
                .HasForeignKey(project => project.OwnerStudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.ToTable("School");
            entity.HasKey(school => school.Id);
            entity.Property(school => school.Name)
                .HasMaxLength(200)
                .IsRequired();
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("Class");
            entity.HasKey(schoolClass => schoolClass.Id);
            entity.Property(schoolClass => schoolClass.Name)
                .HasMaxLength(200)
                .IsRequired();
            entity.HasOne(schoolClass => schoolClass.School)
                .WithMany(school => school.Classes)
                .HasForeignKey(schoolClass => schoolClass.ShoolId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Student");
            entity.HasKey(student => student.Id);
            entity.Property(student => student.UserName)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(student => student.PasswordHash)
                .IsRequired();
            entity.HasIndex(student => student.UserName)
                .IsUnique();
            entity.HasOne(student => student.Class)
                .WithMany(schoolClass => schoolClass.Students)
                .HasForeignKey(student => student.ClassId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
