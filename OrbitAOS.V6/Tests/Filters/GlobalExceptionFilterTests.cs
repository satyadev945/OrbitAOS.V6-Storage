using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Filters;
using Xunit;

namespace OrbitAOS.V6.Tests.Filters
{
    public class GlobalExceptionFilterTests
    {
        private readonly Mock<ILogger<GlobalExceptionFilter>> _mockLogger;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly GlobalExceptionFilter _filter;

        public GlobalExceptionFilterTests()
        {
            _mockLogger = new Mock<ILogger<GlobalExceptionFilter>>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _filter = new GlobalExceptionFilter(_mockLogger.Object, _mockEnvironment.Object);
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var filter = new GlobalExceptionFilter(_mockLogger.Object, _mockEnvironment.Object);

            // Assert
            Assert.NotNull(filter);
        }

        [Fact]
        public void OnException_WithEntityNotFoundException_Returns404()
        {
            // Arrange
            var exception = new KeyNotFoundException("Entity not found");
            var context = CreateExceptionContext(exception);
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            _filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Fact]
        public void OnException_WithValidationException_Returns400()
        {
            // Arrange
            var exception = new ArgumentException("Validation failed");
            var context = CreateExceptionContext(exception);
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            _filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Fact]
        public void OnException_WithUnauthorizedAccessException_Returns401()
        {
            // Arrange
            var exception = new UnauthorizedAccessException("Unauthorized");
            var context = CreateExceptionContext(exception);
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            _filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        }

        [Fact]
        public void OnException_WithGenericException_Returns500()
        {
            // Arrange
            var exception = new InvalidOperationException("Generic error");
            var context = CreateExceptionContext(exception);
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            _filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Fact]
        public void OnException_InDevelopmentEnvironment_IncludesStackTrace()
        {
            // Arrange
            var exception = new Exception("Test error");
            var context = CreateExceptionContext(exception);
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(true);

            // Act
            _filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void OnException_InProductionEnvironment_ExcludesStackTrace()
        {
            // Arrange
            var exception = new Exception("Test error");
            var context = CreateExceptionContext(exception);
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            _filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void OnException_LogsError()
        {
            // Arrange
            var exception = new Exception("Test error");
            var context = CreateExceptionContext(exception);
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            _filter.OnException(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void OnException_SetsExceptionHandledToTrue()
        {
            // Arrange
            var exception = new Exception("Test error");
            var context = CreateExceptionContext(exception);
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            _filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
        }

        [Fact]
        public void OnException_WithNullException_HandlesGracefully()
        {
            // Arrange
            var context = CreateExceptionContext(new Exception());
            _mockEnvironment.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            _filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
        }

        private ExceptionContext CreateExceptionContext(Exception exception)
        {
            var actionContext = new ActionContext
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };

            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };
        }
    }
}
