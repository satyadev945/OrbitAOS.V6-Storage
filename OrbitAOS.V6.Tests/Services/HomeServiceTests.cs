using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Services
{
    /// <summary>
    /// Unit tests for HomeService – Application layer.
    /// </summary>
    public class HomeServiceTests
    {
        private readonly Mock<ILogger<HomeService>> _loggerMock;
        private readonly HomeService _service;

        public HomeServiceTests()
        {
            _loggerMock = new Mock<ILogger<HomeService>>();
            _service = new HomeService(_loggerMock.Object);
        }

        [Fact]
        public async Task GetWelcomeMessageAsync_ReturnsNonEmptyString()
        {
            // Act
            var result = await _service.GetWelcomeMessageAsync();

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetWelcomeMessageAsync_ContainsOrbitAOS()
        {
            // Act
            var result = await _service.GetWelcomeMessageAsync();

            // Assert
            result.Should().Contain("OrbitAOS");
        }

        [Fact]
        public async Task IsHealthyAsync_ReturnsTrue()
        {
            // Act
            var result = await _service.IsHealthyAsync();

            // Assert
            result.Should().BeTrue();
        }
    }
}
