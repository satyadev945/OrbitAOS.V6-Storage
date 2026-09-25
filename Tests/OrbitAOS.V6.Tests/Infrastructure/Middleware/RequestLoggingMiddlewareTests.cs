using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Infrastructure.Middleware;
using System.Net;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure.Middleware
{
    /// <summary>
    /// Unit tests for RequestLoggingMiddleware – Infrastructure layer.
    /// Tests middleware invocation, logging behavior, and exception propagation.
    /// </summary>
    public class RequestLoggingMiddlewareTests
    {
        private readonly Mock<ILogger<RequestLoggingMiddleware>> _loggerMock;

        public RequestLoggingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
        }

        // ─── Constructor ────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            // Arrange
            RequestDelegate next = ctx => Task.CompletedTask;

            // Act
            var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);

            // Assert
            middleware.Should().NotBeNull();
        }

        // ─── InvokeAsync – happy path ────────────────────────────────────────────

        [Fact]
        public async Task InvokeAsync_CallsNextMiddleware()
        {
            // Arrange
            var nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
            var context = CreateHttpContext("GET", "/test");

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task InvokeAsync_LogsRequestInformation()
        {
            // Arrange
            RequestDelegate next = ctx => Task.CompletedTask;
            var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
            var context = CreateHttpContext("GET", "/home");

            // Act
            await middleware.InvokeAsync(context);

            // Assert
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
        public async Task InvokeAsync_WithPostRequest_CallsNextAndLogs()
        {
            // Arrange
            var nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
            var context = CreateHttpContext("POST", "/api/items");

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            nextCalled.Should().BeTrue();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // ─── InvokeAsync – exception propagation ────────────────────────────────

        [Fact]
        public async Task InvokeAsync_WhenNextThrows_StillLogsAndRethrows()
        {
            // Arrange
            RequestDelegate next = ctx => throw new InvalidOperationException("Pipeline error");
            var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
            var context = CreateHttpContext("GET", "/error");

            // Act
            var act = async () => await middleware.InvokeAsync(context);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Pipeline error");

            // Logging should still happen in the finally block
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
        public async Task InvokeAsync_WhenNextThrowsTaskCanceledException_StillLogs()
        {
            // Arrange
            RequestDelegate next = ctx => throw new TaskCanceledException("Cancelled");
            var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
            var context = CreateHttpContext("GET", "/cancel");

            // Act
            var act = async () => await middleware.InvokeAsync(context);

            // Assert
            await act.Should().ThrowAsync<TaskCanceledException>();
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // ─── InvokeAsync – various HTTP methods ──────────────────────────────────

        [Theory]
        [InlineData("GET", "/")]
        [InlineData("POST", "/api/data")]
        [InlineData("PUT", "/api/items/1")]
        [InlineData("DELETE", "/api/items/1")]
        [InlineData("PATCH", "/api/items/1")]
        public async Task InvokeAsync_WithVariousHttpMethods_LogsOnce(string method, string path)
        {
            // Arrange
            RequestDelegate next = ctx => Task.CompletedTask;
            var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
            var context = CreateHttpContext(method, path);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // ─── Helper ─────────────────────────────────────────────────────────────

        private static DefaultHttpContext CreateHttpContext(string method, string path)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = path;
            return context;
        }
    }
}
