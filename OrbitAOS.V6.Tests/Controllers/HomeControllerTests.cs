// ============================================================
// Tests/Controllers/HomeControllerTests.cs
// Unit Tests – Web / Presentation Layer
//
// Tests the HomeController using:
//   • Moq to mock IHomeService and ILogger<HomeController>
//   • xUnit as the test framework
//   • FluentAssertions for readable assertions
// ============================================================

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers;

/// <summary>
/// Unit tests for <see cref="HomeController"/>.
/// </summary>
public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly Mock<IHomeService> _homeServiceMock;
    private readonly HomeController _sut; // System Under Test

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _homeServiceMock = new Mock<IHomeService>();

        _sut = new HomeController(_loggerMock.Object, _homeServiceMock.Object);

        // Provide a minimal HttpContext so HttpContext.TraceIdentifier works
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    // ------------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange
        var logger = new Mock<ILogger<HomeController>>();
        var service = new Mock<IHomeService>();

        // Act
        var controller = new HomeController(logger.Object, service.Object);

        // Assert
        controller.Should().NotBeNull();
    }

    // ------------------------------------------------------------------
    // Index
    // ------------------------------------------------------------------

    [Fact]
    public async Task Index_ReturnsViewResult()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("Welcome!");

        // Act
        var result = await _sut.Index(CancellationToken.None);

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Index_SetsWelcomeMessageInViewData()
    {
        // Arrange
        const string expectedMessage = "Welcome to OrbitAOS!";
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMessage);

        // Act
        await _sut.Index(CancellationToken.None);

        // Assert
        _sut.ViewData["WelcomeMessage"].Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Index_CallsHomeServiceOnce()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("Hello");

        // Act
        await _sut.Index(CancellationToken.None);

        // Assert
        _homeServiceMock.Verify(
            s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Index_WithEmptyMessage_SetsEmptyViewData()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        // Act
        await _sut.Index(CancellationToken.None);

        // Assert
        _sut.ViewData["WelcomeMessage"].Should().Be(string.Empty);
    }

    [Fact]
    public async Task Index_WithCancellationToken_PassesTokenToService()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(token))
            .ReturnsAsync("Cancelled test");

        // Act
        await _sut.Index(token);

        // Assert
        _homeServiceMock.Verify(
            s => s.GetWelcomeMessageAsync(token),
            Times.Once);
    }

    [Fact]
    public async Task Index_ViewResult_HasNullModel()
    {
        // Arrange
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("Hello");

        // Act
        var result = await _sut.Index(CancellationToken.None) as ViewResult;

        // Assert
        result!.Model.Should().BeNull();
    }

    [Fact]
    public async Task Index_WithLongMessage_SetsFullMessageInViewData()
    {
        // Arrange
        var longMessage = new string('A', 1000);
        _homeServiceMock
            .Setup(s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(longMessage);

        // Act
        await _sut.Index(CancellationToken.None);

        // Assert
        _sut.ViewData["WelcomeMessage"].Should().Be(longMessage);
    }

    // ------------------------------------------------------------------
    // Privacy
    // ------------------------------------------------------------------

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _sut.Privacy();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Privacy_ViewResult_HasNullModel()
    {
        // Act
        var result = _sut.Privacy() as ViewResult;

        // Assert
        result!.Model.Should().BeNull();
    }

    [Fact]
    public void Privacy_DoesNotCallHomeService()
    {
        // Act
        _sut.Privacy();

        // Assert
        _homeServiceMock.Verify(
            s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ------------------------------------------------------------------
    // Error
    // ------------------------------------------------------------------

    [Fact]
    public void Error_ReturnsViewResultWithErrorViewModel()
    {
        // Act
        var result = _sut.Error();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().BeOfType<ErrorViewModel>();
    }

    [Fact]
    public void Error_ModelHasRequestId()
    {
        // Act
        var result = _sut.Error() as ViewResult;

        // Assert
        var model = result!.Model as ErrorViewModel;
        model.Should().NotBeNull();
        // TraceIdentifier is set by DefaultHttpContext – just verify it is not null
        model!.RequestId.Should().NotBeNull();
    }

    [Fact]
    public void Error_ModelIsNotNull()
    {
        // Act
        var result = _sut.Error() as ViewResult;

        // Assert
        result!.Model.Should().NotBeNull();
    }

    [Fact]
    public void Error_DoesNotCallHomeService()
    {
        // Act
        _sut.Error();

        // Assert
        _homeServiceMock.Verify(
            s => s.GetWelcomeMessageAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void Error_ErrorViewModel_ShowRequestId_WhenRequestIdSet()
    {
        // Act
        var result = _sut.Error() as ViewResult;
        var model = result!.Model as ErrorViewModel;

        // Assert – DefaultHttpContext provides a non-empty TraceIdentifier
        if (!string.IsNullOrEmpty(model!.RequestId))
        {
            model.ShowRequestId.Should().BeTrue();
        }
    }
}
