// ============================================================
// Infrastructure/Extensions/ServiceCollectionExtensions.cs
// Clean Architecture – Infrastructure Layer
//
// Registers all Infrastructure-layer services:
//   • EF Core DbContext (SQL Server via Microsoft.EntityFrameworkCore.SqlServer)
//   • ASP.NET Core Identity
//   • Generic repository / Unit-of-Work
//
// Migration notes:
//   • UseNpgsql() replaced with UseSqlServer()
//   • AppContext.SetSwitch for Npgsql legacy timestamp removed
//   • Connection string format updated to SQL Server DSN style
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
    /// Registers EF Core (SQL Server), Identity, and repository implementations.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="configuration">Application configuration (appsettings.json).</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ----------------------------------------------------------
        // 1. EF Core – SQL Server (Microsoft.EntityFrameworkCore.SqlServer)
        //    Connection string format: Server=...;Database=...;...
        //    (see appsettings.json: Server=localhost;Database=OrbitAOS_V6_Net8;...)
        // ----------------------------------------------------------
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                {
                    // Retry on transient SQL Server failures (cloud-friendly)
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    // Store EF Core migration history in default dbo schema
                    sqlServerOptions.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
                });
        });

        // ----------------------------------------------------------
        // 2. ASP.NET Core Identity
        //    ApplicationUser (custom) + AddRoles<IdentityRole>
        //    for full role-management support
        // ----------------------------------------------------------
        services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                // Keep confirmed-account requirement from legacy app
                options.SignIn.RequireConfirmedAccount = true;

                // Password policy
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
