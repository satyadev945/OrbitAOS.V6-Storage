// ============================================================
// Tests/Services/HomeServiceTests.cs
// Unit tests for HomeService (Application layer)
// ============================================================
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Services;
using FluentAssertions;

namespace OrbitAOS.V6.Tests.Services;

/// <summary>
/// Unit tests for HomeService.
/// Tests the Application layer service in isolation.
/// </summary>
public class HomeServiceTests
{
    private readonly Mock<ILogger<HomeService>> _mockLogger;
    private readonly HomeService _service;

    public HomeServiceTests()
    {
        _mockLogger = new Mock<ILogger<HomeService>>();
        _service = new HomeService(_mockLogger.Object);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ReturnsNonEmptyString()
    {
        // Act
        var result = await _service.GetWelcomeMessageAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ReturnsConsistentMessage()
    {
        // Act
        var result1 = await _service.GetWelcomeMessageAsync(CancellationToken.None);
        var result2 = await _service.GetWelcomeMessageAsync(CancellationToken.None);

        // Assert - deterministic output
        result1.Should().Be(result2);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ContainsOrbitAOS()
    {
        // Act
        var result = await _service.GetWelcomeMessageAsync(CancellationToken.None);

        // Assert
        result.Should().Contain("OrbitAOS");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        var act = () => new HomeService(null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }
}
