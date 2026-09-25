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
    /// Unit tests for HomeController.
    /// Tests controller actions in isolation using mocked dependencies.
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
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithWelcomeMessage()
        {
            // Arrange
            const string expectedMessage = "Welcome to OrbitAOS";
            _homeServiceMock
                .Setup(s => s.GetWelcomeMessageAsync())
                .ReturnsAsync(expectedMessage);

            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)result;
            viewResult.ViewData["WelcomeMessage"].Should().Be(expectedMessage);
        }

        [Fact]
        public async Task Index_CallsHomeService_Once()
        {
            // Arrange
            _homeServiceMock
                .Setup(s => s.GetWelcomeMessageAsync())
                .ReturnsAsync("Test message");

            // Act
            await _controller.Index();

            // Assert
            _homeServiceMock.Verify(s => s.GetWelcomeMessageAsync(), Times.Once);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Error_ReturnsViewResult_WithErrorViewModel()
        {
            // Arrange – simulate HttpContext for TraceIdentifier
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _controller.Error();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)result;
            viewResult.Model.Should().BeOfType<ErrorViewModel>();
        }

        [Fact]
        public void Error_ReturnsErrorViewModel_WithRequestId()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as ErrorViewModel;
            model.Should().NotBeNull();
            model!.RequestId.Should().Be("test-trace-id");
            model.ShowRequestId.Should().BeTrue();
        }
    }
}
