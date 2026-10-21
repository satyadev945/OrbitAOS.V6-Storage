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
}
