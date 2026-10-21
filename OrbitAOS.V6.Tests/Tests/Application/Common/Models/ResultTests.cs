// =============================================================================
// Tests/Application/Common/Models/ResultTests.cs
// xUnit Tests for Result and Result<T> classes
// =============================================================================

using OrbitAOS.V6.Application.Common.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Application.Common.Models
{
    public class ResultTests
    {
        // -----------------------------------------------------------------------
        // Result (non-generic) tests
        // -----------------------------------------------------------------------

        [Fact]
        public void Success_ReturnsResultWithSucceededTrue()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Success_ReturnsResultWithEmptyErrors()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.NotNull(result.Errors);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Failure_WithEnumerable_ReturnsResultWithSucceededFalse()
        {
            // Arrange
            var errors = new List<string> { "Error 1", "Error 2" };

            // Act
            var result = Result.Failure(errors);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public void Failure_WithEnumerable_ReturnsResultWithErrors()
        {
            // Arrange
            var errors = new List<string> { "Error 1", "Error 2" };

            // Act
            var result = Result.Failure(errors);

            // Assert
            Assert.Equal(errors, result.Errors);
        }

        [Fact]
        public void Failure_WithSingleString_ReturnsResultWithSucceededFalse()
        {
            // Act
            var result = Result.Failure("Single error");

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public void Failure_WithSingleString_ReturnsResultWithOneError()
        {
            // Arrange
            const string errorMessage = "Single error message";

            // Act
            var result = Result.Failure(errorMessage);

            // Assert
            Assert.Single(result.Errors);
            Assert.Contains(errorMessage, result.Errors);
        }

        [Fact]
        public void Failure_WithEmptyEnumerable_ReturnsResultWithSucceededFalse()
        {
            // Act
            var result = Result.Failure(Enumerable.Empty<string>());

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public void Failure_WithEmptyString_ReturnsResultWithSucceededFalse()
        {
            // Act
            var result = Result.Failure(string.Empty);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public void Success_ErrorsIsNotNull()
        {
            // Act
            var result = Result.Success();

            // Assert
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public void Failure_WithMultipleErrors_ContainsAllErrors()
        {
            // Arrange
            var errors = new[] { "Error A", "Error B", "Error C" };

            // Act
            var result = Result.Failure(errors);

            // Assert
            Assert.Equal(3, result.Errors.Count());
        }
    }

    public class ResultGenericTests
    {
        // -----------------------------------------------------------------------
        // Result<T> (generic) tests
        // -----------------------------------------------------------------------

        [Fact]
        public void Success_WithValue_ReturnsResultWithSucceededTrue()
        {
            // Act
            var result = Result<int>.Success(42);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Success_WithValue_ReturnsResultWithCorrectValue()
        {
            // Act
            var result = Result<int>.Success(42);

            // Assert
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public void Success_WithStringValue_ReturnsCorrectValue()
        {
            // Act
            var result = Result<string>.Success("hello");

            // Assert
            Assert.Equal("hello", result.Value);
        }

        [Fact]
        public void Success_WithObjectValue_ReturnsCorrectValue()
        {
            // Arrange
            var obj = new { Name = "Test" };

            // Act
            var result = Result<object>.Success(obj);

            // Assert
            Assert.Equal(obj, result.Value);
        }

        [Fact]
        public void Failure_WithEnumerable_ReturnsResultWithSucceededFalse()
        {
            // Arrange
            var errors = new List<string> { "Error 1" };

            // Act
            var result = Result<int>.Failure(errors);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public void Failure_WithEnumerable_ValueIsDefault()
        {
            // Arrange
            var errors = new List<string> { "Error 1" };

            // Act
            var result = Result<int>.Failure(errors);

            // Assert
            Assert.Equal(default, result.Value);
        }

        [Fact]
        public void Failure_WithSingleString_ReturnsResultWithSucceededFalse()
        {
            // Act
            var result = Result<string>.Failure("error");

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public void Failure_WithSingleString_ContainsError()
        {
            // Arrange
            const string errorMessage = "Something went wrong";

            // Act
            var result = Result<string>.Failure(errorMessage);

            // Assert
            Assert.Single(result.Errors);
            Assert.Contains(errorMessage, result.Errors);
        }

        [Fact]
        public void Failure_WithEnumerable_ContainsAllErrors()
        {
            // Arrange
            var errors = new[] { "Err1", "Err2" };

            // Act
            var result = Result<string>.Failure(errors);

            // Assert
            Assert.Equal(2, result.Errors.Count());
        }

        [Fact]
        public void Success_ErrorsIsEmpty()
        {
            // Act
            var result = Result<int>.Success(1);

            // Assert
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Failure_NullableReferenceType_ValueIsNull()
        {
            // Act
            var result = Result<string>.Failure("error");

            // Assert
            Assert.Null(result.Value);
        }
    }
}
