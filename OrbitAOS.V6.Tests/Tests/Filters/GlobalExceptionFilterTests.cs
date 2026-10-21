// =============================================================================
// Tests/Filters/GlobalExceptionFilterTests.cs
// xUnit Tests for GlobalExceptionFilter class
// =============================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Filters;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Filters
{
    // Fake IHostEnvironment that allows controlling IsDevelopment() via EnvironmentName
    public class FakeHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Production";
        public string ApplicationName { get; set; } = "TestApp";
        public string ContentRootPath { get; set; } = "/";
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }

    public class GlobalExceptionFilterTests
    {
        private readonly Mock<ILogger<GlobalExceptionFilter>> _loggerMock;

        public GlobalExceptionFilterTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionFilter>>();
        }

        private static ExceptionContext CreateExceptionContext(Exception exception)
        {
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            routeData.Values["controller"] = "Home";
            routeData.Values["action"] = "Index";
            var actionContext = new ActionContext(
                httpContext,
                routeData,
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };
        }

        [Fact]
        public void GlobalExceptionFilter_CanBeInstantiated()
        {
            // Arrange
            var env = new FakeHostEnvironment { EnvironmentName = "Development" };
            var metadataProvider = new EmptyModelMetadataProvider();

            // Act
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);

            // Assert
            Assert.NotNull(filter);
        }

        [Fact]
        public void GlobalExceptionFilter_ImplementsIAsyncExceptionFilter()
        {
            // Arrange
            var env = new FakeHostEnvironment { EnvironmentName = "Development" };
            var metadataProvider = new EmptyModelMetadataProvider();

            // Act
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);

            // Assert
            Assert.IsAssignableFrom<IAsyncExceptionFilter>(filter);
        }

        [Fact]
        public async Task OnExceptionAsync_InDevelopment_DoesNotSetResult()
        {
            // Arrange
            var env = new FakeHostEnvironment { EnvironmentName = "Development" };
            var metadataProvider = new EmptyModelMetadataProvider();
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);
            var exceptionContext = CreateExceptionContext(new Exception("Test exception"));

            // Act
            await filter.OnExceptionAsync(exceptionContext);

            // Assert - in development, result should NOT be set
            Assert.Null(exceptionContext.Result);
            Assert.False(exceptionContext.ExceptionHandled);
        }

        [Fact]
        public async Task OnExceptionAsync_InProduction_SetsViewResult()
        {
            // Arrange
            var env = new FakeHostEnvironment { EnvironmentName = "Production" };
            var metadataProvider = new EmptyModelMetadataProvider();
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);
            var exceptionContext = CreateExceptionContext(new Exception("Production error"));

            // Act
            await filter.OnExceptionAsync(exceptionContext);

            // Assert - in production, result should be set and exception handled
            Assert.NotNull(exceptionContext.Result);
            Assert.True(exceptionContext.ExceptionHandled);
            Assert.IsType<ViewResult>(exceptionContext.Result);
        }

        [Fact]
        public async Task OnExceptionAsync_InProduction_ViewNameIsError()
        {
            // Arrange
            var env = new FakeHostEnvironment { EnvironmentName = "Production" };
            var metadataProvider = new EmptyModelMetadataProvider();
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);
            var exceptionContext = CreateExceptionContext(new InvalidOperationException("Invalid op"));

            // Act
            await filter.OnExceptionAsync(exceptionContext);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(exceptionContext.Result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public async Task OnExceptionAsync_InProduction_ViewDataContainsErrorViewModel()
        {
            // Arrange
            var env = new FakeHostEnvironment { EnvironmentName = "Production" };
            var metadataProvider = new EmptyModelMetadataProvider();
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);
            var exceptionContext = CreateExceptionContext(new Exception("Error"));

            // Act
            await filter.OnExceptionAsync(exceptionContext);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(exceptionContext.Result);
            Assert.IsType<OrbitAOS.V6.Models.ErrorViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task OnExceptionAsync_LogsError()
        {
            // Arrange
            var env = new FakeHostEnvironment { EnvironmentName = "Development" };
            var metadataProvider = new EmptyModelMetadataProvider();
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);
            var exception = new Exception("Logged exception");
            var exceptionContext = CreateExceptionContext(exception);

            // Act
            await filter.OnExceptionAsync(exceptionContext);

            // Assert - logger should have been called with LogLevel.Error
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
        public async Task OnExceptionAsync_InStaging_SetsViewResult()
        {
            // Arrange - Staging is not Development, so should behave like Production
            var env = new FakeHostEnvironment { EnvironmentName = "Staging" };
            var metadataProvider = new EmptyModelMetadataProvider();
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);
            var exceptionContext = CreateExceptionContext(new Exception("Staging error"));

            // Act
            await filter.OnExceptionAsync(exceptionContext);

            // Assert
            Assert.NotNull(exceptionContext.Result);
            Assert.True(exceptionContext.ExceptionHandled);
        }

        [Fact]
        public async Task OnExceptionAsync_WithNullRouteValues_DoesNotThrow()
        {
            // Arrange
            var env = new FakeHostEnvironment { EnvironmentName = "Development" };
            var metadataProvider = new EmptyModelMetadataProvider();
            var filter = new GlobalExceptionFilter(_loggerMock.Object, metadataProvider, env);

            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData(); // no controller/action values
            var actionContext = new ActionContext(
                httpContext,
                routeData,
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new Exception("No route values")
            };

            // Act & Assert
            var ex = await Record.ExceptionAsync(() => filter.OnExceptionAsync(exceptionContext));
            Assert.Null(ex);
        }
    }
}
