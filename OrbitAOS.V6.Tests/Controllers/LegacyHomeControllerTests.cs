// ============================================================
// Tests/Controllers/LegacyHomeControllerTests.cs
// Unit tests for legacy Controllers/HomeController — xUnit + Moq
// ============================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers;

public class LegacyHomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly Mock<IHomeService> _homeServiceMock;
    private readonly HomeController _controller;

    public LegacyHomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _homeServiceMock = new Mock<IHomeService>();

        _controller = new HomeController(_loggerMock.Object, _homeServiceMock.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Assert
        Assert.NotNull(_controller);
    }

    // ── Index ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Index_ReturnsViewResult()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("Welcome");

        // Act
        var result = await _controller.Index(CancellationToken.None);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Index_ReturnsHomeViewModel_WithCorrectMessage()
    {
        // Arrange
        const string expectedMessage = "Welcome to OrbitAOS";
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessage);

        // Act
        var result = await _controller.Index(CancellationToken.None);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<HomeViewModel>(viewResult.Model);
        Assert.Equal(expectedMessage, model.WelcomeMessage);
    }

    [Fact]
    public async Task Index_CallsHomeService_Once()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("msg");

        // Act
        await _controller.Index(CancellationToken.None);

        // Assert
        _homeServiceMock.Verify(
            s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Index_ViewModel_GeneratedAt_IsRecentUtc()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("msg");

        // Act
        var result = await _controller.Index(CancellationToken.None);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<HomeViewModel>(viewResult.Model);
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.True(model.GeneratedAt >= before && model.GeneratedAt <= after);
    }

    // ── Privacy ──────────────────────────────────────────────────────────────

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    // ── Error ────────────────────────────────────────────────────────────────

    [Fact]
    public void Error_ReturnsViewResult()
    {
        // Act
        var result = _controller.Error();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ReturnsErrorViewModel()
    {
        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ErrorViewModel>(viewResult.Model);
    }

    [Fact]
    public void Error_SetsRequestId_FromTraceIdentifier()
    {
        // Arrange
        const string traceId = "legacy-trace-id-001";
        _controller.ControllerContext.HttpContext.TraceIdentifier = traceId;

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.Equal(traceId, model.RequestId);
    }

    [Fact]
    public async Task Index_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service failure"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Index(CancellationToken.None));
    }
}
