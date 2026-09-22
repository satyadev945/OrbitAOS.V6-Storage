using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Identity;
using OrbitAOS.Infrastructure.Services;

namespace OrbitAOS.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services with the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Infrastructure layer services including EF Core 8 and repositories.
    /// Replaces the legacy net6.0 ApplicationDbContext registration and upgrades all packages to 8.x.
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register EF Core 8 DbContext with SQL Server provider
        // MigrationsAssembly points to OrbitAOS.Infrastructure where migrations live
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly(
                    typeof(ApplicationDbContext).Assembly.GetName().Name)));

        // Register developer exception page filter for EF Core (development only)
        services.AddDatabaseDeveloperPageExceptionFilter();

        // Register generic repository (open generic registration)
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register specific repositories
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        return services;
    }
}
