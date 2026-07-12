using GestaoProjetos.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoProjetos.Api.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Issue> Issues { get; set; } = null!;
    public DbSet<TimeLog> TimeLogs { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Attachment> Attachments { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Project
        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(150);
            entity.Property(p => p.Description).HasMaxLength(1000);
        });

        // Issue
        modelBuilder.Entity<Issue>(entity =>
        {
            entity.Property(i => i.Title).HasMaxLength(250);
            entity.Property(i => i.Description).HasMaxLength(4000);
            entity.Property(i => i.RequestedBy).HasMaxLength(150);

            entity.HasOne(i => i.Project)
                  .WithMany(p => p.Issues)
                  .HasForeignKey(i => i.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.AssignedToUser)
                  .WithMany(u => u.AssignedIssues)
                  .HasForeignKey(i => i.AssignedToUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // TimeLog
        modelBuilder.Entity<TimeLog>(entity =>
        {
            entity.Property(tl => tl.HoursSpent).HasPrecision(18, 2);
            entity.Property(tl => tl.WorkDescription).HasMaxLength(1000);

            entity.HasOne(tl => tl.Issue)
                  .WithMany(i => i.TimeLogs)
                  .HasForeignKey(tl => tl.IssueId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tl => tl.User)
                  .WithMany(u => u.TimeLogs)
                  .HasForeignKey(tl => tl.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(c => c.Content).HasMaxLength(2000);

            entity.HasOne(c => c.Issue)
                  .WithMany(i => i.Comments)
                  .HasForeignKey(c => c.IssueId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.User)
                  .WithMany(u => u.Comments)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Attachment
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.Property(a => a.FileName).HasMaxLength(250);
            entity.Property(a => a.FilePath).HasMaxLength(1000);
            entity.Property(a => a.UploadedBy).HasMaxLength(150);

            entity.HasOne(a => a.Issue)
                  .WithMany(i => i.Attachments)
                  .HasForeignKey(a => a.IssueId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(al => al.FieldChanged).HasMaxLength(100);
            entity.Property(al => al.OldValue).HasMaxLength(1000);
            entity.Property(al => al.NewValue).HasMaxLength(1000);

            entity.HasOne(al => al.Issue)
                  .WithMany(i => i.AuditLogs)
                  .HasForeignKey(al => al.IssueId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(al => al.User)
                  .WithMany() // Log references User but User doesn't need AuditLogs collection
                  .HasForeignKey(al => al.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
