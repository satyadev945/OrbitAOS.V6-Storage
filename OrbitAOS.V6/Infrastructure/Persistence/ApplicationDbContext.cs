// =============================================================================
// Infrastructure/Persistence/ApplicationDbContext.cs
// Clean Architecture: Infrastructure Layer - EF Core 8 DbContext
//
// Migration Notes:
//   - Upgraded from EF Core 7 to EF Core 8
//   - Implements IApplicationDbContext interface (Clean Architecture)
//   - Inherits IdentityDbContext for ASP.NET Core Identity support
//   - Replaces legacy EF6 DbContext with EF Core 8 equivalent
//   - Adds audit field auto-population (CreatedAt, UpdatedAt)
//   - Supports soft-delete query filters
// =============================================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Application.Common.Interfaces;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Infrastructure.Persistence
{
    /// <summary>
    /// EF Core 8 application database context.
    /// Inherits from IdentityDbContext to support ASP.NET Core Identity.
    /// Implements IApplicationDbContext for Clean Architecture dependency inversion.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ---------------------------------------------------------------
        // Add DbSet<T> properties here as domain entities are created.
        // Example:
        //   public DbSet<Product> Products => Set<Product>();
        //   public DbSet<Order> Orders => Set<Order>();
        // ---------------------------------------------------------------

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all entity configurations from the current assembly
            // This scans for classes implementing IEntityTypeConfiguration<T>
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // ---------------------------------------------------------------
            // Global query filters for soft-delete pattern
            // Entities inheriting BaseEntity with IsDeleted=true are excluded
            // from all queries automatically.
            // ---------------------------------------------------------------
            // Example (uncomment when entities are added):
            // builder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        }

        /// <summary>
        /// Overrides SaveChangesAsync to automatically populate audit fields.
        /// Sets CreatedAt on new entities and UpdatedAt on modified entities.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Auto-populate audit fields for BaseEntity-derived entities
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
