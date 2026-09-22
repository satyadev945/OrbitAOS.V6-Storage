using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Application.Services;

namespace OrbitAOS.Application.Common;

/// <summary>
/// Extension methods for registering Application layer services with the DI container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Application layer services.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileService, UserProfileService>();
        return services;
    }
}
