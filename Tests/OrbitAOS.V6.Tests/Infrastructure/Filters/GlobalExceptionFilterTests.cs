using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.V6.Infrastructure.Filters;
using System.Collections.Generic;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure.Filters
{
    /// <summary>
    /// Unit tests for GlobalExceptionFilter – Infrastructure layer.
    /// Tests exception handling behavior in development and production environments.
    /// </summary>
    public class GlobalExceptionFilterTests
    {
        private readonly Mock<ILogger<GlobalExceptionFilter>> _loggerMock;
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly GlobalExceptionFilter _filter;

        public GlobalExceptionFilterTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionFilter>>();
            _envMock = new Mock<IWebHostEnvironment>();
            _filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);
        }

        // ─── Constructor ────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidDependencies_CreatesInstance()
        {
            // Arrange & Act
            var filter = new GlobalExceptionFilter(_loggerMock.Object, _envMock.Object);

            // Assert
            filter.Should().NotBeNull();
        }

        // ─── OnException – Development environment ──────────────────────────────

        [Fact]
        public void OnException_InDevelopment_DoesNotSetResult()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Development");
            var context = CreateExceptionContext(new InvalidOperationException("Test error"));

            // Act
            _filter.OnException(context);

            // Assert
            context.Result.Should().BeNull();
        }

        [Fact]
        public void OnException_InDevelopment_ExceptionNotHandled()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Development");
            var context = CreateExceptionContext(new Exception("Dev error"));

            // Act
            _filter.OnException(context);

            // Assert
            context.ExceptionHandled.Should().BeFalse();
        }

        // ─── OnException – Production environment ───────────────────────────────

        [Fact]
        public void OnException_InProduction_SetsRedirectResult()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Production");
            var context = CreateExceptionContext(new Exception("Prod error"));

            // Act
            _filter.OnException(context);

            // Assert
            context.Result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void OnException_InProduction_RedirectsToHomeError()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Production");
            var context = CreateExceptionContext(new Exception("Prod error"));

            // Act
            _filter.OnException(context);

            // Assert
            var redirect = context.Result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Error");
            redirect.ControllerName.Should().Be("Home");
        }

        [Fact]
        public void OnException_InProduction_MarksExceptionAsHandled()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Production");
            var context = CreateExceptionContext(new Exception("Prod error"));

            // Act
            _filter.OnException(context);

            // Assert
            context.ExceptionHandled.Should().BeTrue();
        }

        [Fact]
        public void OnException_InProduction_LogsError()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Production");
            var exception = new InvalidOperationException("Critical failure");
            var context = CreateExceptionContext(exception);

            // Act
            _filter.OnException(context);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // ─── OnException – Staging environment ──────────────────────────────────

        [Fact]
        public void OnException_InStaging_SetsRedirectResult()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Staging");
            var context = CreateExceptionContext(new Exception("Staging error"));

            // Act
            _filter.OnException(context);

            // Assert
            context.Result.Should().BeOfType<RedirectToActionResult>();
            context.ExceptionHandled.Should().BeTrue();
        }

        // ─── Various exception types ─────────────────────────────────────────────

        [Fact]
        public void OnException_WithNullReferenceException_InProduction_HandlesGracefully()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Production");
            var context = CreateExceptionContext(new NullReferenceException("Null ref"));

            // Act
            _filter.OnException(context);

            // Assert
            context.ExceptionHandled.Should().BeTrue();
        }

        [Fact]
        public void OnException_WithArgumentException_InProduction_HandlesGracefully()
        {
            // Arrange
            _envMock.Setup(e => e.EnvironmentName).Returns("Production");
            var context = CreateExceptionContext(new ArgumentException("Bad argument"));

            // Act
            _filter.OnException(context);

            // Assert
            context.ExceptionHandled.Should().BeTrue();
        }

        // ─── Helper ─────────────────────────────────────────────────────────────

        private static ExceptionContext CreateExceptionContext(Exception exception)
        {
            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };
        }
    }
}
