// ============================================================
// Tests/Web/Middleware/RequestLoggingMiddlewareTests.cs
// Unit tests for RequestLoggingMiddleware — xUnit + Moq
// ============================================================

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Web.Middleware;
using Xunit;

namespace OrbitAOS.V6.Tests.Web.Middleware;

public class RequestLoggingMiddlewareTests
{
    private readonly Mock<ILogger<RequestLoggingMiddleware>> _loggerMock;

    public RequestLoggingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
    }

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Arrange
        RequestDelegate next = ctx => Task.CompletedTask;

        // Act
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);

        // Assert
        Assert.NotNull(middleware);
    }

    // ── InvokeAsync ──────────────────────────────────────────────────────────

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
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/test";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithGetRequest_CompletesSuccessfully()
    {
        // Arrange
        RequestDelegate next = ctx => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/home";

        // Act & Assert — should not throw
        await middleware.InvokeAsync(context);
    }

    [Fact]
    public async Task InvokeAsync_WithPostRequest_CompletesSuccessfully()
    {
        // Arrange
        RequestDelegate next = ctx => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/data";

        // Act & Assert — should not throw
        await middleware.InvokeAsync(context);
    }

    [Fact]
    public async Task InvokeAsync_WhenNextThrows_ExceptionPropagates()
    {
        // Arrange
        RequestDelegate next = ctx => throw new InvalidOperationException("Pipeline error");
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/error";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => middleware.InvokeAsync(context));
    }

    [Fact]
    public async Task InvokeAsync_WhenNextThrows_StillLogsRequest()
    {
        // Arrange
        RequestDelegate next = ctx => throw new InvalidOperationException("Pipeline error");
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/error";

        // Act
        try
        {
            await middleware.InvokeAsync(context);
        }
        catch (InvalidOperationException)
        {
            // Expected
        }

        // Assert — logger should have been called (in finally block)
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_LogsInformation_AfterRequest()
    {
        // Arrange
        RequestDelegate next = ctx => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/home";
        context.Response.StatusCode = 200;

        // Act
        await middleware.InvokeAsync(context);

        // Assert — verify logger was called with Information level
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithCancellationToken_CompletesSuccessfully()
    {
        // Arrange
        RequestDelegate next = ctx => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/";

        // Act & Assert
        await middleware.InvokeAsync(context);
    }

    [Fact]
    public void Middleware_IsSealed()
    {
        // Assert
        Assert.True(typeof(RequestLoggingMiddleware).IsSealed);
    }
}
