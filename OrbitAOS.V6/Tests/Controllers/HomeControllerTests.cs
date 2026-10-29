using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Controllers;
using OrbitAOS.V6.Models;
using System.Diagnostics;
using Xunit;

namespace OrbitAOS.V6.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithValidLogger_CreatesInstance()
        {
            // Arrange & Act
            var controller = new HomeController(_mockLogger.Object);

            // Assert
            Assert.NotNull(controller);
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
        public void Index_ReturnsNonNullResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.NotNull(result);
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
        public void Privacy_ReturnsNonNullResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Error_ReturnsViewResult()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _controller.Error();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        [Fact]
        public void Error_SetsRequestIdFromTraceIdentifier()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("test-trace-id", model.RequestId);
        }

        [Fact]
        public void Error_HasResponseCacheAttribute()
        {
            // Arrange
            var methodInfo = typeof(HomeController).GetMethod(nameof(HomeController.Error));

            // Act
            var attributes = methodInfo?.GetCustomAttributes(typeof(ResponseCacheAttribute), false);

            // Assert
            Assert.NotNull(attributes);
            Assert.Single(attributes);
            var attribute = attributes[0] as ResponseCacheAttribute;
            Assert.NotNull(attribute);
            Assert.Equal(0, attribute.Duration);
            Assert.Equal(ResponseCacheLocation.None, attribute.Location);
            Assert.True(attribute.NoStore);
        }

        [Fact]
        public void Error_ReturnsNonNullModel()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public void Error_ModelHasRequestId()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
        }

        [Theory]
        [InlineData("trace-id-1")]
        [InlineData("trace-id-2")]
        [InlineData("custom-trace-123")]
        public void Error_UsesProvidedTraceIdentifier(string traceId)
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = traceId;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal(traceId, model.RequestId);
        }

        [Fact]
        public void HomeController_InheritsFromController()
        {
            // Assert
            Assert.IsAssignableFrom<Controller>(_controller);
        }

        [Fact]
        public void Index_CanBeCalledMultipleTimes()
        {
            // Act
            var result1 = _controller.Index();
            var result2 = _controller.Index();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.IsType<ViewResult>(result1);
            Assert.IsType<ViewResult>(result2);
        }

        [Fact]
        public void Privacy_CanBeCalledMultipleTimes()
        {
            // Act
            var result1 = _controller.Privacy();
            var result2 = _controller.Privacy();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.IsType<ViewResult>(result1);
            Assert.IsType<ViewResult>(result2);
        }
    }
}
