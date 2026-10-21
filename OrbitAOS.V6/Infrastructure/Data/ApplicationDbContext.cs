// ============================================================
// Infrastructure/Data/ApplicationDbContext.cs
// EF Core DbContext — Clean Architecture Infrastructure Layer
// Upgraded from net6.0 EF Core 6 to EF Core 8
// ============================================================

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Identity;

namespace OrbitAOS.V6.Infrastructure.Data;

/// <summary>
/// Main application DbContext.
/// Inherits from <see cref="IdentityDbContext{TUser}"/> to include ASP.NET Core Identity tables.
/// Upgraded to use <see cref="ApplicationUser"/> instead of the base <see cref="Microsoft.AspNetCore.Identity.IdentityUser"/>.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ── Domain entity DbSets ─────────────────────────────────────────────────
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    // Add additional DbSets here as domain entities are created, e.g.:
    // public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── AuditLog configuration ───────────────────────────────────────────
        builder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(256);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(256);
            entity.Property(e => e.EntityId).HasMaxLength(450);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
            // Soft-delete global query filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ── ApplicationUser configuration ────────────────────────────────────
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.DisplayName).HasMaxLength(256);
        });

        // ── EF Core 8: Apply all IEntityTypeConfiguration<T> from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically set audit timestamps.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<Domain.Common.BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
