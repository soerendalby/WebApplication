using Microsoft.EntityFrameworkCore;
using WebApplication.Models;

namespace WebApplication.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<StudyProgram> StudyPrograms => Set<StudyProgram>();
    public DbSet<Approver> Approvers => Set<Approver>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<Approval> Approvals => Set<Approval>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<StudyProgram>()
            .HasIndex(sp => sp.Name)
            .IsUnique();

        modelBuilder.Entity<Approver>()
            .HasIndex(a => new { a.UserId, a.StudyProgramId })
            .IsUnique();

        modelBuilder.Entity<Approval>()
            .HasIndex(a => a.RequestId)
            .IsUnique();

        modelBuilder.Entity<Request>()
            .HasIndex(r => new { r.StudyProgramId, r.Status });

        modelBuilder.Entity<Request>()
            .HasIndex(r => new { r.UserId, r.SubmittedAt });

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.Timestamp);

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.Action);
    }
}
