// ============================================================
// Tests/Application/ServiceCollectionExtensionsTests.cs
// Unit Tests – Application Layer (DI Extensions)
//
// Tests the AddApplication() extension method.
// ============================================================

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.V6.Application.Extensions;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Application;

/// <summary>
/// Unit tests for <see cref="OrbitAOS.V6.Application.Extensions.ServiceCollectionExtensions"/>.
/// </summary>
public class ApplicationServiceCollectionExtensionsTests
{
    // ------------------------------------------------------------------
    // AddApplication
    // ------------------------------------------------------------------

    [Fact]
    public void AddApplication_RegistersIHomeService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        // Assert
        var service = provider.GetService<IHomeService>();
        service.Should().NotBeNull();
    }

    [Fact]
    public void AddApplication_IHomeService_IsRegisteredAsHomeService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        // Assert
        var service = provider.GetService<IHomeService>();
        service.Should().BeOfType<HomeService>();
    }

    [Fact]
    public void AddApplication_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddApplication();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddApplication_IHomeService_IsRegisteredAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApplication();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IHomeService));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddApplication_CalledTwice_DoesNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        var act = () =>
        {
            services.AddApplication();
            services.AddApplication();
        };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void AddApplication_ScopedService_DifferentScopesReturnDifferentInstances()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        var provider = services.BuildServiceProvider();

        // Act
        IHomeService? service1, service2;
        using (var scope1 = provider.CreateScope())
        {
            service1 = scope1.ServiceProvider.GetService<IHomeService>();
        }
        using (var scope2 = provider.CreateScope())
        {
            service2 = scope2.ServiceProvider.GetService<IHomeService>();
        }

        // Assert
        service1.Should().NotBeNull();
        service2.Should().NotBeNull();
        // Different scope instances should be different objects
        service1.Should().NotBeSameAs(service2);
    }
}
