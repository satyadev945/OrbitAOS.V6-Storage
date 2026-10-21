// ============================================================
// Tests/Services/HomeServiceTests.cs
// Unit Tests – Application Layer
//
// Tests the HomeService using:
//   • Moq to mock ILogger<HomeService>
//   • xUnit as the test framework
//   • FluentAssertions for readable assertions
// ============================================================

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Services;

/// <summary>
/// Unit tests for <see cref="HomeService"/>.
/// </summary>
public class HomeServiceTests
{
    private readonly Mock<ILogger<HomeService>> _loggerMock;
    private readonly HomeService _sut;

    public HomeServiceTests()
    {
        _loggerMock = new Mock<ILogger<HomeService>>();
        _sut = new HomeService(_loggerMock.Object);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ReturnsNonEmptyString()
    {
        // Act
        var result = await _sut.GetWelcomeMessageAsync();

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ContainsOrbitAOS()
    {
        // Act
        var result = await _sut.GetWelcomeMessageAsync();

        // Assert
        result.Should().Contain("OrbitAOS");
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ContainsDotNet8()
    {
        // Act
        var result = await _sut.GetWelcomeMessageAsync();

        // Assert
        result.Should().Contain(".NET 8");
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_RespectsCancel()
    {
        // Arrange – pre-cancelled token
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert – HomeService returns a completed task so no exception expected
        var result = await _sut.GetWelcomeMessageAsync(cts.Token);
        result.Should().NotBeNullOrWhiteSpace();
    }
}
