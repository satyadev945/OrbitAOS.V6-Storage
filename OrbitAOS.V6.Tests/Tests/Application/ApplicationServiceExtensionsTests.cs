// =============================================================================
// Tests/Application/ApplicationServiceExtensionsTests.cs
// xUnit Tests for ApplicationServiceExtensions class
// =============================================================================

using Microsoft.Extensions.DependencyInjection;
using OrbitAOS.V6.Application;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Application
{
    public class ApplicationServiceExtensionsTests
    {
        [Fact]
        public void AddApplicationServices_ReturnsServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddApplicationServices();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void AddApplicationServices_ReturnsSameServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddApplicationServices();

            // Assert
            Assert.Same(services, result);
        }

        [Fact]
        public void AddApplicationServices_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddApplicationServices();

            // Assert - services should have been registered (count > 0)
            Assert.True(services.Count > 0);
        }

        [Fact]
        public void AddApplicationServices_CanBeCalledMultipleTimes()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert - should not throw
            var ex = Record.Exception(() =>
            {
                services.AddApplicationServices();
                services.AddApplicationServices();
            });

            Assert.Null(ex);
        }

        [Fact]
        public void AddApplicationServices_WithNullServices_ThrowsException()
        {
            // Arrange
            IServiceCollection? services = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => services!.AddApplicationServices());
        }
    }
}
