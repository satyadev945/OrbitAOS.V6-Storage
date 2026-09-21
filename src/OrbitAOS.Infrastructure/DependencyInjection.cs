using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Repositories;

namespace OrbitAOS.Infrastructure
{
    /// <summary>
    /// Extension methods for registering Infrastructure layer services.
    /// Centralizes all dependency injection registrations for the infrastructure layer.
    /// Follows the AddInfrastructure() pattern for clean DI registration.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {
            // Register EF Core 8 DbContext with SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register ASP.NET Core Identity with explicit password policy
            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // Explicit password complexity rules (issue-15 fix)
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // Register generic repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register user profile repository
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            // Register application services
            services.AddScoped<IUserProfileService, UserProfileService>();

            return services;
        }
    }
}
