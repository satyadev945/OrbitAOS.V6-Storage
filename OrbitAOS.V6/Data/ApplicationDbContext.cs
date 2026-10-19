// ============================================================
// Data/ApplicationDbContext.cs
// EF Core 8 DbContext - Infrastructure / Data layer
// Upgraded from EF Core 6.0.19 to EF Core 8.0.0
// ============================================================
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data;

/// <summary>
/// Application database context.
/// Inherits from IdentityDbContext to include ASP.NET Core Identity tables.
/// 
/// Migration notes:
/// - EF6 ObjectContext / DbContext → EF Core 8 DbContext
/// - Connection string moved from web.config to appsettings.json
/// - Fluent API configuration preferred over data annotations
/// - Async methods (SaveChangesAsync) used throughout
/// - No more Database.SetInitializer() - use Migrations instead
/// </summary>
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // -------------------------------------------------------
    // Add your application DbSet<T> properties here.
    // Example:
    //   public DbSet<Product> Products => Set<Product>();
    //   public DbSet<Order> Orders => Set<Order>();
    // -------------------------------------------------------

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // -------------------------------------------------------
        // EF Core 8 Fluent API configuration
        // Replaces: EF6 EntityTypeConfiguration<T> classes and
        //           [DatabaseGenerated], [ForeignKey] attributes
        //           scattered across legacy model classes.
        // -------------------------------------------------------

        // Example: Configure a custom entity
        // builder.Entity<Product>(entity =>
        // {
        //     entity.HasKey(e => e.Id);
        //     entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        //     entity.HasIndex(e => e.Name).IsUnique();
        // });

        // Apply all IEntityTypeConfiguration<T> classes from the Data assembly
        // builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically set audit timestamps.
    /// Replaces legacy interceptors or manual timestamp assignment in controllers.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-set CreatedAt / UpdatedAt on BaseEntity-derived entities
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Entities.BaseEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Entities.BaseEntity)entry.Entity;
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            entity.UpdatedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
