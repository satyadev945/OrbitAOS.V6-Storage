using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Application.Services
{
    /// <summary>
    /// Unit tests for HomeService – Application layer.
    /// Tests all public methods with various scenarios.
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

        // ─── Constructor ────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidLogger_CreatesInstance()
        {
            // Arrange & Act
            var service = new HomeService(_loggerMock.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullLogger_DoesNotThrow()
        {
            // Arrange & Act – HomeService does not guard against null logger (DI handles it)
            var act = () => new HomeService(null!);
            // Assert – no exception is thrown; the service is created successfully
            act.Should().NotThrow();
        }

        // ─── GetWelcomeMessageAsync ─────────────────────────────────────────────

        [Fact]
        public async Task GetWelcomeMessageAsync_ReturnsNonNullString()
        {
            // Act
            var result = await _service.GetWelcomeMessageAsync();

            // Assert
            result.Should().NotBeNull();
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
        public async Task GetWelcomeMessageAsync_ContainsDotNet8Reference()
        {
            // Act
            var result = await _service.GetWelcomeMessageAsync();

            // Assert
            result.Should().Contain(".NET 8");
        }

        [Fact]
        public async Task GetWelcomeMessageAsync_ReturnsCompletedTask()
        {
            // Act
            var task = _service.GetWelcomeMessageAsync();

            // Assert
            task.Should().NotBeNull();
            var result = await task;
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetWelcomeMessageAsync_CalledMultipleTimes_ReturnsSameMessage()
        {
            // Act
            var result1 = await _service.GetWelcomeMessageAsync();
            var result2 = await _service.GetWelcomeMessageAsync();

            // Assert
            result1.Should().Be(result2);
        }

        [Fact]
        public async Task GetWelcomeMessageAsync_LogsInformation()
        {
            // Act
            await _service.GetWelcomeMessageAsync();

            // Assert – verify that LogInformation was called at least once
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        // ─── IsHealthyAsync ─────────────────────────────────────────────────────

        [Fact]
        public async Task IsHealthyAsync_ReturnsTrue()
        {
            // Act
            var result = await _service.IsHealthyAsync();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsHealthyAsync_ReturnsCompletedTask()
        {
            // Act
            var task = _service.IsHealthyAsync();

            // Assert
            task.Should().NotBeNull();
            var result = await task;
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsHealthyAsync_CalledMultipleTimes_AlwaysReturnsTrue()
        {
            // Act
            var result1 = await _service.IsHealthyAsync();
            var result2 = await _service.IsHealthyAsync();
            var result3 = await _service.IsHealthyAsync();

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeTrue();
        }

        [Fact]
        public async Task IsHealthyAsync_DoesNotThrow()
        {
            // Act
            var act = async () => await _service.IsHealthyAsync();

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
