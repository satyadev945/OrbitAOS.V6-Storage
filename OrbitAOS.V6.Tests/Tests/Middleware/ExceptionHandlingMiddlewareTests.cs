// =============================================================================
// Tests/Middleware/ExceptionHandlingMiddlewareTests.cs
// xUnit Tests for ExceptionHandlingMiddleware and its extension
// =============================================================================

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Middleware;
using System.Net;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        private readonly Mock<IHostEnvironment> _environmentMock;

        public ExceptionHandlingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _environmentMock = new Mock<IHostEnvironment>();
        }

        [Fact]
        public void ExceptionHandlingMiddleware_CanBeInstantiated()
        {
            // Arrange
            RequestDelegate next = ctx => Task.CompletedTask;

            // Act
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);

            // Assert
            Assert.NotNull(middleware);
        }

        [Fact]
        public async Task InvokeAsync_NoException_CallsNext()
        {
            // Arrange
            var nextCalled = false;
            RequestDelegate next = ctx =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);
            var context = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task InvokeAsync_WithArgumentNullException_SetsBadRequestStatus()
        {
            // Arrange
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");
            RequestDelegate next = ctx => throw new ArgumentNullException("param");
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/test";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithUnauthorizedAccessException_SetsUnauthorizedStatus()
        {
            // Arrange
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");
            RequestDelegate next = ctx => throw new UnauthorizedAccessException();
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/test";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithKeyNotFoundException_SetsNotFoundStatus()
        {
            // Arrange
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");
            RequestDelegate next = ctx => throw new KeyNotFoundException();
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/test";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_WithGenericException_SetsInternalServerErrorStatus()
        {
            // Arrange
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");
            RequestDelegate next = ctx => throw new Exception("Generic error");
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/test";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_ApiPath_SetsJsonContentType()
        {
            // Arrange
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");
            RequestDelegate next = ctx => throw new Exception("error");
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/values";

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal("application/json", context.Response.ContentType);
        }

        [Fact]
        public async Task InvokeAsync_NonApiPath_RedirectsToErrorPage()
        {
            // Arrange
            _environmentMock.Setup(e => e.EnvironmentName).Returns("Production");
            RequestDelegate next = ctx => throw new Exception("error");
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/Home/Index";

            // Act
            await middleware.InvokeAsync(context);

            // Assert - redirect sets 302 status
            Assert.Equal(302, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_NoException_StatusCodeUnchanged()
        {
            // Arrange
            RequestDelegate next = ctx => Task.CompletedTask;
            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _environmentMock.Object);
            var context = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert - default status code is 200
            Assert.Equal(200, context.Response.StatusCode);
        }
    }

    public class ExceptionHandlingMiddlewareExtensionsTests
    {
        [Fact]
        public void UseCustomExceptionHandling_ReturnsApplicationBuilder()
        {
            // Arrange
            var appBuilderMock = new Mock<IApplicationBuilder>();
            appBuilderMock
                .Setup(a => a.Use(It.IsAny<Func<RequestDelegate, RequestDelegate>>()))
                .Returns(appBuilderMock.Object);

            // Act
            var result = appBuilderMock.Object.UseCustomExceptionHandling();

            // Assert
            Assert.NotNull(result);
        }
    }
}
