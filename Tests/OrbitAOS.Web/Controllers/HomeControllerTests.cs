using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;
using Xunit;

namespace OrbitAOS.Web.Controllers
{
    /// <summary>
    /// Comprehensive unit tests for <see cref="HomeController"/>.
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_loggerMock.Object);
        }

        // ── Constructor Tests ────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidLogger_CreatesInstance()
        {
            // Arrange & Act
            var controller = new HomeController(_loggerMock.Object);

            // Assert
            Assert.NotNull(controller);
        }

        // ── Index Tests ──────────────────────────────────────────────────────

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithNullModel()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Model);
        }

        [Fact]
        public void Index_LogsInformation()
        {
            // Act
            _controller.Index();

            // Assert - verify logger was called
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void Index_ReturnsIActionResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        // ── Privacy Tests ────────────────────────────────────────────────────

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult_WithNullModel()
        {
            // Act
            var result = _controller.Privacy() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Model);
        }

        [Fact]
        public void Privacy_ReturnsIActionResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        // ── Error Tests ──────────────────────────────────────────────────────

        [Fact]
        public void Error_ReturnsViewResult()
        {
            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewResultWithErrorViewModel()
        {
            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ErrorViewModel>(result.Model);
        }

        [Fact]
        public void Error_WithNullHttpContext_ReturnsViewWithNullRequestId()
        {
            // Arrange - no HttpContext set
            // Act
            var result = _controller.Error() as ViewResult;
            var model = result?.Model as ErrorViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.Null(model.RequestId);
        }

        [Fact]
        public void Error_WithHttpContext_ReturnsViewWithTraceIdentifier()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "trace-id-123";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error() as ViewResult;
            var model = result?.Model as ErrorViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.Equal("trace-id-123", model.RequestId);
        }

        [Fact]
        public void Error_ReturnsIActionResult()
        {
            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
