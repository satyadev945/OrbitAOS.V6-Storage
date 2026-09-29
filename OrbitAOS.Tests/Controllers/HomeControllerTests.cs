using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Web.Controllers;
using OrbitAOS.Web.Models;
using Xunit;

namespace OrbitAOS.Tests.Controllers
{
    /// <summary>
    /// Unit tests for <see cref="HomeController"/>.
    /// </summary>
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly HomeController _controller;

        /// <summary>
        /// Initializes test fixtures for HomeController tests.
        /// </summary>
        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_loggerMock.Object);
        }

        /// <summary>
        /// Verifies that the Index action returns a ViewResult.
        /// </summary>
        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Verifies that the Privacy action returns a ViewResult.
        /// </summary>
        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Verifies that the Error action returns a ViewResult with an ErrorViewModel.
        /// </summary>
        [Fact]
        public void Error_ReturnsViewResultWithErrorViewModel()
        {
            // Arrange - HttpContext is null in unit tests, so RequestId will be null
            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        /// <summary>
        /// Verifies that ErrorViewModel.ShowRequestId returns false when RequestId is null.
        /// </summary>
        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsFalse_WhenRequestIdIsNull()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = null };

            // Assert
            Assert.False(model.ShowRequestId);
        }

        /// <summary>
        /// Verifies that ErrorViewModel.ShowRequestId returns true when RequestId is set.
        /// </summary>
        [Fact]
        public void ErrorViewModel_ShowRequestId_ReturnsTrue_WhenRequestIdIsSet()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "test-request-id" };

            // Assert
            Assert.True(model.ShowRequestId);
        }
    }
}
