// ============================================================
// Tests/Web/Filters/GlobalExceptionFilterTests.cs
// Unit tests for GlobalExceptionFilter — xUnit + Moq
// ============================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Web.Filters;
using Xunit;

namespace OrbitAOS.V6.Tests.Web.Filters;

public class GlobalExceptionFilterTests
{
    private readonly Mock<ILogger<GlobalExceptionFilter>> _loggerMock;
    private readonly Mock<IWebHostEnvironment> _envMock;

    public GlobalExceptionFilterTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionFilter>>();
        _envMock = new Mock<IWebHostEnvironment>();
    }

    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        routeData.Values["controller"] = "Home";
        routeData.Values["action"] = "Index";

        var actionDescriptor = new ActionDescriptor();
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidDependencies_CreatesInstance()
    {
        // Act
        var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);

        // Assert
        Assert.NotNull(filter);
    }

    // ── OnException — Development ────────────────────────────────────────────

    [Fact]
    public void OnException_InDevelopment_DoesNotSetResult()
    {
        // Arrange
        _envMock.Setup(e => e.EnvironmentName).Returns("Development");
        // IsDevelopment() is an extension method that checks EnvironmentName == "Development"
        var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);
        var context = CreateExceptionContext(new InvalidOperationException("Test error"));

        // Act
        filter.OnException(context);

        // Assert — in Development, result should NOT be set (let default error page handle it)
        Assert.False(context.ExceptionHandled);
    }

    // ── OnException — Production ─────────────────────────────────────────────

    [Fact]
    public void OnException_InProduction_SetsExceptionHandled()
    {
        // Arrange
        _envMock.Setup(e => e.EnvironmentName).Returns("Production");

        var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);
        var context = CreateExceptionContext(new InvalidOperationException("Test error"));

        // Act
        filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_InProduction_SetsViewResult()
    {
        // Arrange
        _envMock.Setup(e => e.EnvironmentName).Returns("Production");

        var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);
        var context = CreateExceptionContext(new InvalidOperationException("Test error"));

        // Act
        filter.OnException(context);

        // Assert
        Assert.IsType<ViewResult>(context.Result);
    }

    [Fact]
    public void OnException_InProduction_ViewResult_HasErrorViewName()
    {
        // Arrange
        _envMock.Setup(e => e.EnvironmentName).Returns("Production");

        var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);
        var context = CreateExceptionContext(new InvalidOperationException("Test error"));

        // Act
        filter.OnException(context);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(context.Result);
        Assert.Equal("~/Views/Shared/Error.cshtml", viewResult.ViewName);
    }

    [Fact]
    public void OnException_LogsError_WithExceptionDetails()
    {
        // Arrange
        _envMock.Setup(e => e.EnvironmentName).Returns("Development");

        var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);
        var exception = new InvalidOperationException("Test error");
        var context = CreateExceptionContext(exception);

        // Act
        filter.OnException(context);

        // Assert — verify logger was called with Error level
        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void OnException_WithNullReferenceException_HandledInProduction()
    {
        // Arrange
        _envMock.Setup(e => e.EnvironmentName).Returns("Production");

        var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);
        var context = CreateExceptionContext(new NullReferenceException("Null ref"));

        // Act
        filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void OnException_WithArgumentException_HandledInProduction()
    {
        // Arrange
        _envMock.Setup(e => e.EnvironmentName).Returns("Production");

        var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);
        var context = CreateExceptionContext(new ArgumentException("Bad arg"));

        // Act
        filter.OnException(context);

        // Assert
        Assert.True(context.ExceptionHandled);
    }

    [Fact]
    public void Filter_IsSealed()
    {
        // Assert
        Assert.True(typeof(GlobalExceptionFilter).IsSealed);
    }
}
