using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;
using System.Diagnostics;
using Xunit;

namespace OrbitAOS.Tests.Controllers
{
    /// <summary>
    /// Unit tests for HomeController.
    /// Tests all action methods using Moq for dependency injection.
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);

            // Set up HttpContext for controller
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
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
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public void Error_SetsRequestId_WhenActivityCurrentIsNull()
        {
            // Arrange - ensure no current activity
            var traceId = "test-trace-id";
            _controller.ControllerContext.HttpContext.TraceIdentifier = traceId;

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal(traceId, model.RequestId);
        }

        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsFalse_WhenRequestIdIsNull()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = null };

            // Assert
            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsTrue_WhenRequestIdIsSet()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "some-id" };

            // Assert
            Assert.True(model.ShowRequestId);
        }
    }
}
