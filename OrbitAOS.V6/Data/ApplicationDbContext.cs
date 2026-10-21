// ============================================================
// Data/ApplicationDbContext.cs
// Clean Architecture – Infrastructure / Data Layer
//
// Migration notes:
//   • EF Core 8.0.0 with SQL Server provider
//   • IdentityDbContext<ApplicationUser> for custom user entity
//   • ApplicationUser is defined in Domain/Entities/ApplicationUser.cs
//   • OnModelCreating: default schema set to "dbo" (SQL Server)
//   • Removed PostgreSQL-specific extensions (uuid-ossp, snake_case)
// ============================================================

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Data;

/// <summary>
/// Primary EF Core database context for OrbitAOS.
/// Inherits from <see cref="IdentityDbContext{TUser}"/> to include
/// all ASP.NET Core Identity tables.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Initialises a new instance of <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="options">EF Core context options (injected by DI).</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ------------------------------------------------------------------
    // Add domain DbSets here as the application grows, for example:
    //   public DbSet<Order> Orders => Set<Order>();
    //   public DbSet<Product> Products => Set<Product>();
    // ------------------------------------------------------------------

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all IEntityTypeConfiguration<T> classes found in this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
