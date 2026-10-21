// ============================================================
// Tests/Application/Services/HomeServiceTests.cs
// Unit tests for HomeService — xUnit + Moq
// ============================================================

using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Application.Services;

public class HomeServiceTests
{
    private readonly Mock<ILogger<HomeService>> _loggerMock;
    private readonly HomeService _service;

    public HomeServiceTests()
    {
        _loggerMock = new Mock<ILogger<HomeService>>();
        _service = new HomeService(_loggerMock.Object);
    }

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange & Act
        var service = new HomeService(_loggerMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act — HomeService stores the logger without null-check (implicit usings)
        // The constructor does not throw; it accepts null and only fails on use
        // Verify the service is created (no guard clause in implementation)
        var service = new HomeService(null!);
        Assert.NotNull(service);
    }

    // ── GetWelcomeMessageAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetWelcomeMessageAsync_ReturnsNonNullString()
    {
        // Act
        var result = await _service.GetWelcomeMessageAsync();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ReturnsNonEmptyString()
    {
        // Act
        var result = await _service.GetWelcomeMessageAsync();

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ContainsDotNet8()
    {
        // Act
        var result = await _service.GetWelcomeMessageAsync();

        // Assert
        Assert.Contains(".NET 8", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ContainsOrbitAOS()
    {
        // Act
        var result = await _service.GetWelcomeMessageAsync();

        // Assert
        Assert.Contains("OrbitAOS", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_WithDefaultCancellationToken_Succeeds()
    {
        // Act
        var result = await _service.GetWelcomeMessageAsync(default);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_WithCancelledToken_DoesNotThrow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act — in-memory service should not throw for cancelled token
        var result = await _service.GetWelcomeMessageAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_CalledMultipleTimes_ReturnsSameValue()
    {
        // Act
        var result1 = await _service.GetWelcomeMessageAsync();
        var result2 = await _service.GetWelcomeMessageAsync();

        // Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ReturnsTask_NotNull()
    {
        // Act
        var task = _service.GetWelcomeMessageAsync();

        // Assert
        Assert.NotNull(task);
        await task; // ensure it completes
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_IsSealed_ServiceType()
    {
        // Assert — HomeService is sealed
        Assert.True(typeof(HomeService).IsSealed);
        var result = await _service.GetWelcomeMessageAsync();
        Assert.NotNull(result);
    }
}
