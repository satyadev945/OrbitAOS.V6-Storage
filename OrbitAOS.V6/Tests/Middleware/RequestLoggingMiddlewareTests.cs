using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Middleware;
using Xunit;

namespace OrbitAOS.V6.Tests.Middleware
{
    public class RequestLoggingMiddlewareTests
    {
        private readonly Mock<ILogger<RequestLoggingMiddleware>> _mockLogger;
        private readonly Mock<RequestDelegate> _mockNext;

        public RequestLoggingMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<RequestLoggingMiddleware>>();
            _mockNext = new Mock<RequestDelegate>();
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(middleware);
        }

        [Fact]
        public async Task InvokeAsync_LogsRequestStart()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            context.Request.Method = "GET";

            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Handling")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeAsync_LogsRequestCompletion()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            context.Request.Method = "GET";
            context.Response.StatusCode = 200;

            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Completed")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeAsync_CallsNextDelegate()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            context.Request.Method = "GET";

            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_LogsElapsedTime()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            context.Request.Method = "GET";

            _mockNext.Setup(n => n(It.IsAny<HttpContext>()))
                .Returns(async () => await Task.Delay(10));

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ms")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeAsync_LogsStatusCode()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            context.Request.Method = "GET";
            context.Response.StatusCode = 404;

            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("404")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeAsync_LogsRequestPath()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/users";
            context.Request.Method = "POST";

            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("/api/users")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeAsync_LogsRequestMethod()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            context.Request.Method = "DELETE";

            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("DELETE")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task InvokeAsync_LogsCompletionEvenWhenExceptionThrown()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = "/test";
            context.Request.Method = "GET";

            _mockNext.Setup(n => n(It.IsAny<HttpContext>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => middleware.InvokeAsync(context));

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Completed")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Theory]
        [InlineData("GET", "/api/test")]
        [InlineData("POST", "/api/users")]
        [InlineData("PUT", "/api/products/1")]
        [InlineData("DELETE", "/api/items/5")]
        public async Task InvokeAsync_HandlesVariousHttpMethods(string method, string path)
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_mockNext.Object, _mockLogger.Object);
            var context = new DefaultHttpContext();
            context.Request.Path = path;
            context.Request.Method = method;

            _mockNext.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(n => n(context), Times.Once);
        }
    }

    public class RequestLoggingMiddlewareExtensionsTests
    {
        [Fact]
        public void UseRequestLogging_ReturnsApplicationBuilder()
        {
            // Arrange
            var mockBuilder = new Mock<IApplicationBuilder>();
            mockBuilder.Setup(b => b.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns(mockBuilder.Object);

            // Act
            var result = RequestLoggingMiddlewareExtensions.UseRequestLogging(mockBuilder.Object);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UseRequestLogging_CallsUseMiddleware()
        {
            // Arrange
            var mockBuilder = new Mock<IApplicationBuilder>();
            mockBuilder.Setup(b => b.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns(mockBuilder.Object);

            // Act
            RequestLoggingMiddlewareExtensions.UseRequestLogging(mockBuilder.Object);

            // Assert
            mockBuilder.Verify(b => b.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()), Times.Once);
        }
    }
}
