using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;

namespace OrbitAOS.Tests.Web;

/// <summary>
/// Unit tests for <see cref="HomeController"/>.
/// Tests the ASP.NET Core MVC controller actions migrated from legacy ASP.NET MVC.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);

        // Set up a minimal HttpContext so TraceIdentifier is available
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public void Index_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Privacy_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ShouldReturnViewResultWithErrorViewModel()
    {
        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.NotNull(model);
    }

    [Fact]
    public void Error_ShouldPopulateRequestId_WhenTraceIdentifierAvailable()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.TraceIdentifier = "test-trace-id";

        // Act
        var result = _controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
        Assert.Equal("test-trace-id", model.RequestId);
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ErrorViewModel_ShowRequestId_ShouldBeFalse_WhenRequestIdIsNull()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = null };

        // Assert
        Assert.False(model.ShowRequestId);
    }

    [Fact]
    public void ErrorViewModel_ShowRequestId_ShouldBeTrue_WhenRequestIdIsSet()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "some-id" };

        // Assert
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void Index_ShouldReturnViewResult_WithNoModel()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model);
    }

    [Fact]
    public void Privacy_ShouldReturnViewResult_WithNoModel()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model);
    }
}
