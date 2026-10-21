// ============================================================
// Tests/Unit/Services/HomeServiceTests.cs
// Unit tests for HomeService — xUnit
// Clean Architecture: Tests live in Tests/Unit/Services/
// ============================================================

using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Unit.Services;

/// <summary>
/// Unit tests for <see cref="HomeService"/>.
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
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ContainsDotNet8()
    {
        // Act
        var result = await _service.GetWelcomeMessageAsync();

        // Assert — message should mention .NET 8
        Assert.Contains(".NET 8", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_RespectsCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert — should not throw for a simple in-memory service
        var result = await _service.GetWelcomeMessageAsync(cts.Token);
        Assert.NotNull(result);
    }
}
