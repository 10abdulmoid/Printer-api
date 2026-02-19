using Microsoft.EntityFrameworkCore;
using PrintQueueService.Domain.Entities;

namespace PrintQueueService.Application.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Printer> Printers => Set<Printer>();
    public DbSet<Queue> Queues => Set<Queue>();
    public DbSet<PrintJob> PrintJobs => Set<PrintJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Printer configuration
        modelBuilder.Entity<Printer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Capabilities).HasMaxLength(500);
            entity.Property(e => e.Status).HasConversion<string>();
        });

        // Queue configuration
        modelBuilder.Entity<Queue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Printer)
                  .WithMany(p => p.Queues)
                  .HasForeignKey(e => e.PrinterId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // PrintJob configuration
        modelBuilder.Entity<PrintJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.SubmittedBy).HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.ErrorMessage).HasMaxLength(500);
            entity.HasOne(e => e.Queue)
                  .WithMany(q => q.PrintJobs)
                  .HasForeignKey(e => e.QueueId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.BaseEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.BaseEntity)entry.Entity;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAtUtc = DateTime.UtcNow;
            }
        }
    }
}
