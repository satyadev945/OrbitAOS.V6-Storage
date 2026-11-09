using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers;

/// <summary>
/// Unit tests for HomeController actions.
/// Validates that all controller actions return the expected IActionResult types
/// after the .NET 6 to .NET 10 migration.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);

        // Set up a minimal HttpContext so TraceIdentifier is available for Error action
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

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
        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(model);
    }

    [Fact]
    public void Error_SetsRequestId_FromHttpContext_WhenActivityIsNull()
    {
        // Arrange: ensure a known TraceIdentifier
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "test-trace-id-123";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.Model);
        // RequestId should be set (either from Activity.Current or TraceIdentifier)
        Assert.NotNull(model.RequestId);
        Assert.NotEmpty(model.RequestId);
    }

    [Fact]
    public void Error_HasResponseCacheAttribute_WithNoStore()
    {
        // Verify the Error action has the correct ResponseCache attribute
        var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));
        Assert.NotNull(methodInfo);

        var attr = methodInfo!.GetCustomAttributes(typeof(ResponseCacheAttribute), false)
            .Cast<ResponseCacheAttribute>()
            .FirstOrDefault();

        Assert.NotNull(attr);
        Assert.Equal(0, attr!.Duration);
        Assert.Equal(ResponseCacheLocation.None, attr.Location);
        Assert.True(attr.NoStore);
    }

    [Fact]
    public void Index_ReturnsViewResult_WithNullModel()
    {
        // Act
        var result = _controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result!.Model);
    }

    [Fact]
    public void Privacy_ReturnsViewResult_WithNullModel()
    {
        // Act
        var result = _controller.Privacy() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Null(result!.Model);
    }
}
