using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers
{
    /// <summary>
    /// Unit tests for HomeController – Web layer.
    /// Tests all controller actions in isolation using mocked dependencies.
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

            // Default HttpContext for all tests
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        // ─── Constructor ────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            // Arrange & Act
            var controller = new HomeController(_loggerMock.Object, _homeServiceMock.Object);

            // Assert
            controller.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullLogger_DoesNotThrow()
        {
            // Arrange & Act – HomeController does not guard against null logger (DI handles it)
            var act = () => new HomeController(null!, _homeServiceMock.Object);
            // Assert – no exception is thrown; the controller is created successfully
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithNullHomeService_DoesNotThrow()
        {
            // Arrange & Act – HomeController does not guard against null service (DI handles it)
            var act = () => new HomeController(_loggerMock.Object, null!);
            // Assert – no exception is thrown; the controller is created successfully
            act.Should().NotThrow();
        }

        // ─── Index action ───────────────────────────────────────────────────────

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Arrange
            _homeServiceMock
                .Setup(s => s.GetWelcomeMessageAsync())
                .ReturnsAsync("Welcome");

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Index_SetsWelcomeMessageInViewData()
        {
            // Arrange
            const string expectedMessage = "Welcome to OrbitAOS";
            _homeServiceMock
                .Setup(s => s.GetWelcomeMessageAsync())
                .ReturnsAsync(expectedMessage);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewData["WelcomeMessage"].Should().Be(expectedMessage);
        }

        [Fact]
        public async Task Index_CallsGetWelcomeMessageAsync_ExactlyOnce()
        {
            // Arrange
            _homeServiceMock
                .Setup(s => s.GetWelcomeMessageAsync())
                .ReturnsAsync("Hello");

            // Act
            await _controller.Index();

            // Assert
            _homeServiceMock.Verify(s => s.GetWelcomeMessageAsync(), Times.Once);
        }

        [Fact]
        public async Task Index_WithEmptyMessage_SetsEmptyStringInViewData()
        {
            // Arrange
            _homeServiceMock
                .Setup(s => s.GetWelcomeMessageAsync())
                .ReturnsAsync(string.Empty);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.ViewData["WelcomeMessage"].Should().Be(string.Empty);
        }

        [Fact]
        public async Task Index_WhenServiceThrows_PropagatesException()
        {
            // Arrange
            _homeServiceMock
                .Setup(s => s.GetWelcomeMessageAsync())
                .ThrowsAsync(new InvalidOperationException("Service failure"));

            // Act
            var act = async () => await _controller.Index();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Service failure");
        }

        // ─── Privacy action ─────────────────────────────────────────────────────

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
            result!.ViewName.Should().BeNull(); // default view
        }

        // ─── Error action ───────────────────────────────────────────────────────

        [Fact]
        public void Error_ReturnsViewResult()
        {
            // Act
            var result = _controller.Error();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Error_ReturnsViewResult_WithErrorViewModelAsModel()
        {
            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.Model.Should().BeOfType<ErrorViewModel>();
        }

        [Fact]
        public void Error_WithTraceIdentifier_SetsRequestIdOnModel()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "trace-abc-123";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error() as ViewResult;
            var model = result!.Model as ErrorViewModel;

            // Assert
            model.Should().NotBeNull();
            model!.RequestId.Should().Be("trace-abc-123");
            model.ShowRequestId.Should().BeTrue();
        }

        [Fact]
        public void Error_WithDefaultHttpContext_ModelHasRequestId()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _controller.Error() as ViewResult;
            var model = result!.Model as ErrorViewModel;

            // Assert
            model.Should().NotBeNull();
            // TraceIdentifier is set by ASP.NET Core runtime; in tests it may be a default value
            model!.RequestId.Should().NotBeNull();
        }

        [Fact]
        public void Error_DoesNotCallHomeService()
        {
            // Act
            _controller.Error();

            // Assert
            _homeServiceMock.VerifyNoOtherCalls();
        }
    }
}
