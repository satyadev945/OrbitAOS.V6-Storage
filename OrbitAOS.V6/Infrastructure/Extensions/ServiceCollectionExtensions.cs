// ============================================================
// Infrastructure/Extensions/ServiceCollectionExtensions.cs
// Clean Architecture – Infrastructure Layer
//
// Registers all Infrastructure-layer services:
//   • EF Core DbContext (SQL Server)
//   • ASP.NET Core Identity
//   • Generic repository / Unit-of-Work
//
// Called from Program.cs: builder.Services.AddInfrastructure(config)
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
    /// Registers EF Core, Identity, and repository implementations.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="configuration">Application configuration (appsettings.json).</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ----------------------------------------------------------
        // 1. EF Core – SQL Server
        //    Migration note: connection string stays in appsettings.json
        //    (was web.config <connectionStrings> in legacy MVC)
        // ----------------------------------------------------------
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found in appsettings.json.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    // Retry on transient SQL Server failures (cloud-friendly)
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

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
