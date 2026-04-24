using Microsoft.EntityFrameworkCore;
using student_projects.Models;

namespace student_projects.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();

    public DbSet<StudentProject> StudentProjects => Set<StudentProject>();

    public DbSet<School> Schools => Set<School>();

    public DbSet<Class> Classes => Set<Class>();

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.UserName)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(user => user.PasswordHash)
                .IsRequired();
            entity.HasIndex(user => user.UserName)
                .IsUnique();
            entity.HasMany(user => user.OwnedProjects)
                .WithOne(project => project.Owner)
                .HasForeignKey(project => project.OwnerUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

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
            entity.HasOne(student => student.Class)
                .WithMany(schoolClass => schoolClass.Students)
                .HasForeignKey(student => student.ClassId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
