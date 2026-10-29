using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

namespace OrbitAOS.V6.Application
{
    /// <summary>
    /// Dependency injection configuration for Application layer
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // MediatR (if using CQRS pattern)
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
