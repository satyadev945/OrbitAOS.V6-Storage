using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Identity;

namespace OrbitAOS.V6.Infrastructure.Data;

/// <summary>
/// Application database context.
/// Inherits from IdentityDbContext with custom ApplicationUser for ASP.NET Core Identity.
/// Migrated from EF6 ObjectContext/DbContext to EF Core 8 DbContext.
/// Uses async operations throughout for .NET 8 best practices.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>DbSet for SampleEntity records.</summary>
    public DbSet<SampleEntity> SampleEntities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure SampleEntity
        builder.Entity<SampleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Description)
                  .HasMaxLength(1000);

            entity.Property(e => e.CreatedBy)
                  .HasMaxLength(256);

            entity.Property(e => e.ModifiedBy)
                  .HasMaxLength(256);

            // Global query filter for soft delete - automatically excludes deleted records
            entity.HasQueryFilter(e => !e.IsDeleted);

            // Indexes for performance
            entity.HasIndex(e => e.Name).HasDatabaseName("IX_SampleEntities_Name");
            entity.HasIndex(e => e.IsDeleted).HasDatabaseName("IX_SampleEntities_IsDeleted");
            entity.HasIndex(e => e.IsActive).HasDatabaseName("IX_SampleEntities_IsActive");
        });

        // Configure ApplicationUser additional properties
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });
    }
}
