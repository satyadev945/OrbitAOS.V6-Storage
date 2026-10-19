// ============================================================
// Tests/Controllers/HomeControllerTests.cs
// Unit tests for HomeController (.NET 8 / xUnit / Moq)
// ============================================================
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using FluentAssertions;

namespace OrbitAOS.V6.Tests.Controllers;

/// <summary>
/// Unit tests for HomeController.
/// Tests controller actions in isolation using mocked dependencies.
/// 
/// Testing approach:
/// - Arrange: Set up mocks and test data
/// - Act: Call the controller action
/// - Assert: Verify the result type and data
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;
    private readonly Mock<IHomeService> _mockHomeService;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
        _mockHomeService = new Mock<IHomeService>();

        _controller = new HomeController(_mockLogger.Object, _mockHomeService.Object);

        // Set up a mock HttpContext for TraceIdentifier access in Error action
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // -------------------------------------------------------
    // Index Action Tests
    // -------------------------------------------------------

    [Fact]
    public async Task Index_ReturnsViewResult()
    {
        // Arrange
        _mockHomeService
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("Welcome to OrbitAOS");

        // Act
        var result = await _controller.Index(CancellationToken.None);

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Index_SetsWelcomeMessageInViewData()
    {
        // Arrange
        const string expectedMessage = "Welcome to OrbitAOS - .NET 8";
        _mockHomeService
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessage);

        // Act
        var result = await _controller.Index(CancellationToken.None) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.ViewData["WelcomeMessage"].Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Index_CallsHomeServiceOnce()
    {
        // Arrange
        _mockHomeService
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("Welcome");

        // Act
        await _controller.Index(CancellationToken.None);

        // Assert
        _mockHomeService.Verify(
            s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // -------------------------------------------------------
    // Privacy Action Tests
    // -------------------------------------------------------

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_ReturnsDefaultView()
    {
        // Act
        var result = _controller.Privacy() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        // ViewName null means the default view name (action name) is used
        result!.ViewName.Should().BeNullOrEmpty();
    }

    // -------------------------------------------------------
    // Error Action Tests
    // -------------------------------------------------------

    [Fact]
    public void Error_ReturnsViewResult()
    {
        // Act
        var result = _controller.Error();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Error_ReturnsErrorViewModelWithRequestId()
    {
        // Arrange
        _controller.ControllerContext.HttpContext.TraceIdentifier = "test-trace-id-123";

        // Act
        var result = _controller.Error() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().BeOfType<ErrorViewModel>();
        var model = (ErrorViewModel)result.Model!;
        model.RequestId.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Error_HasNoCacheResponseCacheAttribute()
    {
        // Verify the [ResponseCache] attribute is applied correctly
        var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));
        var attribute = methodInfo!
            .GetCustomAttributes(typeof(ResponseCacheAttribute), false)
            .FirstOrDefault() as ResponseCacheAttribute;

        attribute.Should().NotBeNull();
        attribute!.Duration.Should().Be(0);
        attribute.NoStore.Should().BeTrue();
        attribute.Location.Should().Be(ResponseCacheLocation.None);
    }

    // -------------------------------------------------------
    // Constructor Tests
    // -------------------------------------------------------

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        var act = () => new HomeController(null!, _mockHomeService.Object);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenHomeServiceIsNull()
    {
        // Act & Assert
        var act = () => new HomeController(_mockLogger.Object, null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("homeService");
    }
}
