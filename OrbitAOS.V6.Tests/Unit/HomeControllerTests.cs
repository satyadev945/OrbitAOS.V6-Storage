using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Web.Controllers;
using OrbitAOS.V6.Web.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Unit;

/// <summary>
/// Unit tests for HomeController.
/// Tests all action methods using Moq for dependency mocking.
/// Follows AAA (Arrange-Act-Assert) pattern.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_mockLogger.Object);

        // Set up HttpContext for TraceIdentifier
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
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_ShouldReturnViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Error_ShouldReturnViewResultWithErrorViewModel()
    {
        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeOfType<ErrorViewModel>();
    }

    [Fact]
    public void Error_ShouldSetRequestId_WhenTraceIdentifierAvailable()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.TraceIdentifier = "test-trace-id";

        // Act
        var result = _controller.Error() as ViewResult;
        var model = result?.Model as ErrorViewModel;

        // Assert
        model.Should().NotBeNull();
        model!.ShowRequestId.Should().BeTrue();
        model.RequestId.Should().Be("test-trace-id");
    }

    [Fact]
    public void Error_ShouldHaveNoCacheResponseCacheAttribute()
    {
        // Arrange
        var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));

        // Act
        var attribute = methodInfo?
            .GetCustomAttributes(typeof(ResponseCacheAttribute), false)
            .FirstOrDefault() as ResponseCacheAttribute;

        // Assert
        attribute.Should().NotBeNull();
        attribute!.Duration.Should().Be(0);
        attribute.NoStore.Should().BeTrue();
    }

    [Fact]
    public void ErrorViewModel_ShowRequestId_ShouldBeFalse_WhenRequestIdIsNull()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = null };

        // Assert
        model.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ErrorViewModel_ShowRequestId_ShouldBeTrue_WhenRequestIdIsSet()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "some-id" };

        // Assert
        model.ShowRequestId.Should().BeTrue();
    }
}
