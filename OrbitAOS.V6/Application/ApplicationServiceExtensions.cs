// =============================================================================
// Application/ApplicationServiceExtensions.cs
// Clean Architecture: Application Layer - Service Registration
//
// Migration Notes:
//   - Centralizes Application layer DI registrations
//   - Called from Program.cs to register MediatR, AutoMapper, validators
//   - Replaces scattered service registrations in legacy App_Start files
// =============================================================================

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OrbitAOS.V6.Application
{
    /// <summary>
    /// Extension methods for registering Application layer services.
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Registers all Application layer services into the DI container.
        /// Includes MediatR handlers, AutoMapper profiles, and FluentValidation validators.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register MediatR for CQRS pattern
            // Scans the current assembly for IRequestHandler<,> implementations
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Register AutoMapper profiles from the current assembly
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register FluentValidation validators from the current assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
