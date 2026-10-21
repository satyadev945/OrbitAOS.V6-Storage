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

    // ------------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidLogger_CreatesInstance()
    {
        // Arrange
        var logger = new Mock<ILogger<HomeService>>();

        // Act
        var service = new HomeService(logger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    // ------------------------------------------------------------------
    // GetWelcomeMessageAsync – return value
    // ------------------------------------------------------------------

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
    public async Task GetWelcomeMessageAsync_ContainsCleanArchitecture()
    {
        // Act
        var result = await _sut.GetWelcomeMessageAsync();

        // Assert
        result.Should().Contain("Clean Architecture");
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ReturnsExactExpectedMessage()
    {
        // Arrange
        const string expected = "Welcome to OrbitAOS – running on .NET 8 with Clean Architecture.";

        // Act
        var result = await _sut.GetWelcomeMessageAsync();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_ReturnTypeIsString()
    {
        // Act
        var result = await _sut.GetWelcomeMessageAsync();

        // Assert
        result.Should().BeOfType<string>();
    }

    // ------------------------------------------------------------------
    // GetWelcomeMessageAsync – CancellationToken handling
    // ------------------------------------------------------------------

    [Fact]
    public async Task GetWelcomeMessageAsync_WithDefaultCancellationToken_Succeeds()
    {
        // Act
        var result = await _sut.GetWelcomeMessageAsync(default);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_WithCancellationTokenNone_Succeeds()
    {
        // Act
        var result = await _sut.GetWelcomeMessageAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_WithPreCancelledToken_StillReturnsMessage()
    {
        // Arrange – pre-cancelled token
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert – HomeService returns a completed task so no exception expected
        var result = await _sut.GetWelcomeMessageAsync(cts.Token);
        result.Should().NotBeNullOrWhiteSpace();
    }

    // ------------------------------------------------------------------
    // GetWelcomeMessageAsync – logging side-effects
    // ------------------------------------------------------------------

    [Fact]
    public async Task GetWelcomeMessageAsync_LogsInformationOnce()
    {
        // Act
        await _sut.GetWelcomeMessageAsync();

        // Assert – verify that LogInformation was called at least once
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    // ------------------------------------------------------------------
    // GetWelcomeMessageAsync – multiple calls
    // ------------------------------------------------------------------

    [Fact]
    public async Task GetWelcomeMessageAsync_CalledMultipleTimes_ReturnsSameMessage()
    {
        // Act
        var result1 = await _sut.GetWelcomeMessageAsync();
        var result2 = await _sut.GetWelcomeMessageAsync();
        var result3 = await _sut.GetWelcomeMessageAsync();

        // Assert
        result1.Should().Be(result2);
        result2.Should().Be(result3);
    }

    [Fact]
    public async Task GetWelcomeMessageAsync_IsAwaitable_ReturnsCompletedTask()
    {
        // Act
        var task = _sut.GetWelcomeMessageAsync();

        // Assert
        task.Should().NotBeNull();
        var result = await task;
        result.Should().NotBeNullOrWhiteSpace();
    }
}
