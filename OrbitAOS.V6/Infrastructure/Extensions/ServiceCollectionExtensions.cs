// ============================================================
// Infrastructure/Extensions/ServiceCollectionExtensions.cs
// Clean Architecture – Infrastructure Layer
//
// Registers all Infrastructure-layer services:
//   • EF Core DbContext (PostgreSQL via Npgsql)
//   • ASP.NET Core Identity
//   • Generic repository / Unit-of-Work
//
// Migration notes (SQL Server → PostgreSQL):
//   • UseSqlServer() replaced with UseNpgsql()
//   • EnableRetryOnFailure() uses Npgsql-compatible overload
//   • AppContext.SetSwitch for legacy timestamp behaviour added
//   • Called from Program.cs: builder.Services.AddInfrastructure(config)
// ============================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Persistence;

namespace OrbitAOS.V6.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering Infrastructure-layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers EF Core (PostgreSQL), Identity, and repository implementations.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="configuration">Application configuration (appsettings.json).</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ----------------------------------------------------------
        // Enable legacy UTC timestamp behaviour for Npgsql.
        // Required so that DateTime values stored without timezone
        // information are treated as UTC (matches SQL Server behaviour).
        // ----------------------------------------------------------
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // ----------------------------------------------------------
        // 1. EF Core – PostgreSQL (Npgsql)
        //    Migration note: UseSqlServer → UseNpgsql
        //    Connection string format changed to PostgreSQL DSN style
        //    (see appsettings.json: Host=...;Port=5432;Database=...;...)
        // ----------------------------------------------------------
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    // Retry on transient PostgreSQL failures (cloud-friendly)
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);

                    // Store EF Core migration history in snake_case table
                    npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
                });

            // Apply snake_case naming convention for all PostgreSQL objects
            options.UseSnakeCaseNamingConvention();
        });

        // ----------------------------------------------------------
        // 2. ASP.NET Core Identity
        //    Migration note:
        //      Legacy: IdentityUser (bare) + AddDefaultIdentity
        //      .NET 8 : ApplicationUser (custom) + AddRoles<IdentityRole>
        //               for full role-management support
        // ----------------------------------------------------------
        services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                // Keep confirmed-account requirement from legacy app
                options.SignIn.RequireConfirmedAccount = true;

                // Password policy (adjust to match legacy web.config settings)
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // ----------------------------------------------------------
        // 3. Generic Repository + Unit-of-Work
        // ----------------------------------------------------------
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
