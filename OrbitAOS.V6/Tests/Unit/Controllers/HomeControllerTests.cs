// ============================================================
// Tests/Unit/Controllers/HomeControllerTests.cs
// Unit tests for HomeController — xUnit + Moq
// Clean Architecture: Tests live in Tests/Unit/Controllers/
// ============================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using System.Diagnostics;
using Xunit;

namespace OrbitAOS.V6.Tests.Unit.Controllers;

/// <summary>
/// Unit tests for <see cref="HomeController"/>.
/// Uses xUnit and Moq. No database or HTTP server required.
/// </summary>
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

        // Set up a default HttpContext so HttpContext.TraceIdentifier works
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ── Index ────────────────────────────────────────────────────────────────

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
        // Null ViewName means the default view (Index.cshtml) is used
        Assert.Null(viewResult.ViewName);
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
    public void Error_ReturnsViewResult_WithErrorViewModel()
    {
        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(model);
    }

    [Fact]
    public void Error_SetsRequestId_FromTraceIdentifier_WhenActivityIsNull()
    {
        // Arrange — ensure no ambient Activity
        var traceId = "test-trace-id-123";
        _controller.ControllerContext.HttpContext.TraceIdentifier = traceId;

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.Equal(traceId, model.RequestId);
    }
}
