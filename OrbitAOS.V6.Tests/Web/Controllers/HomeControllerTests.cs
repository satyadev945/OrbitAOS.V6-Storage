// ============================================================
// Tests/Web/Controllers/HomeControllerTests.cs
// Unit tests for Web/Controllers/HomeController — xUnit + Moq
// ============================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Models;
using OrbitAOS.V6.Web.Controllers;
using Xunit;

namespace OrbitAOS.V6.Tests.Web.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly Mock<IHomeService> _homeServiceMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
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
    public async Task Index_ReturnsViewResult_WithHomeViewModel()
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

    [Fact]
    public async Task Index_CallsHomeService_ExactlyOnce()
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
    public async Task Index_ReturnsDefaultViewName()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("msg");

        // Act
        var result = await _controller.Index(CancellationToken.None);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName); // null = default view (Index.cshtml)
    }

    [Fact]
    public async Task Index_WhenServiceReturnsEmptyString_ViewModelHasEmptyMessage()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        // Act
        var result = await _controller.Index(CancellationToken.None);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<HomeViewModel>(viewResult.Model);
        Assert.Equal(string.Empty, model.WelcomeMessage);
    }

    [Fact]
    public async Task Index_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Index(CancellationToken.None));
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

    [Fact]
    public void Privacy_ReturnsDefaultViewName()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewName);
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
    public void Error_ReturnsViewResult_WithErrorViewModel()
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
        const string traceId = "test-trace-id-xyz";
        _controller.ControllerContext.HttpContext.TraceIdentifier = traceId;

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.Equal(traceId, model.RequestId);
    }

    [Fact]
    public void Error_ErrorViewModel_ShowRequestId_IsTrue_WhenTraceIdSet()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.TraceIdentifier = "some-trace-id";

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.True(model.ShowRequestId);
    }
}
