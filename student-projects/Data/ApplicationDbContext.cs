using Microsoft.EntityFrameworkCore;
using student_projects.Models;

namespace student_projects.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();

    public DbSet<StudentProject> StudentProjects => Set<StudentProject>();

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
    }
}
