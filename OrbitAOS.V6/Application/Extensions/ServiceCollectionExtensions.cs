// ============================================================
// Application/Extensions/ServiceCollectionExtensions.cs
// Clean Architecture – Application Layer
//
// Registers all Application-layer services with the DI container.
// Called from Program.cs: builder.Services.AddApplication()
// ============================================================

using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Application.Extensions;

/// <summary>
/// Extension methods for registering Application-layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all Application-layer services (business logic, validators, etc.).
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IHomeService, HomeService>();

        // Future: Register MediatR, FluentValidation, AutoMapper here
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        // services.AddAutoMapper(Assembly.GetExecutingAssembly());
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
