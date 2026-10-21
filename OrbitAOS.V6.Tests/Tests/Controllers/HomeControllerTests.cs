// =============================================================================
// Tests/Controllers/HomeControllerTests.cs
// xUnit Tests for HomeController class
// =============================================================================

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _mediatorMock = new Mock<IMediator>();
            _controller = new HomeController(_loggerMock.Object, _mediatorMock.Object);

            // Setup HttpContext for TraceIdentifier
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void HomeController_CanBeInstantiated()
        {
            // Assert
            Assert.NotNull(_controller);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_ReturnsViewWithNullModel()
        {
            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Model);
        }

        [Fact]
        public async Task Index_LogsInformation()
        {
            // Act
            await _controller.Index();

            // Assert
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
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
        public void Privacy_ReturnsViewWithNullModel()
        {
            // Act
            var result = _controller.Privacy() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Model);
        }

        [Fact]
        public void Error_ReturnsViewResult()
        {
            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ErrorViewModel>(result.Model);
        }

        [Fact]
        public void Error_ErrorViewModelHasRequestId()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.TraceIdentifier = "test-trace-id";

            // Act
            var result = _controller.Error() as ViewResult;
            var model = result?.Model as ErrorViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.NotNull(model.RequestId);
        }

        [Fact]
        public void Error_ErrorViewModelRequestIdIsNotEmpty()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.TraceIdentifier = "trace-123";

            // Act
            var result = _controller.Error() as ViewResult;
            var model = result?.Model as ErrorViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public async Task Index_DoesNotThrow()
        {
            // Act & Assert
            var ex = await Record.ExceptionAsync(() => _controller.Index());
            Assert.Null(ex);
        }

        [Fact]
        public void Privacy_DoesNotThrow()
        {
            // Act & Assert
            var ex = Record.Exception(() => _controller.Privacy());
            Assert.Null(ex);
        }

        [Fact]
        public void Error_DoesNotThrow()
        {
            // Act & Assert
            var ex = Record.Exception(() => _controller.Error());
            Assert.Null(ex);
        }

        [Fact]
        public void HomeController_IsControllerSubclass()
        {
            // Assert
            Assert.IsAssignableFrom<Controller>(_controller);
        }
    }
}
