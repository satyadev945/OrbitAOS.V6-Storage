using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Models
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void Constructor_CreatesInstance()
        {
            // Arrange & Act
            var model = new ErrorViewModel();

            // Assert
            Assert.NotNull(model);
        }

        [Fact]
        public void RequestId_CanBeSet()
        {
            // Arrange
            var model = new ErrorViewModel();
            var requestId = "test-request-id";

            // Act
            model.RequestId = requestId;

            // Assert
            Assert.Equal(requestId, model.RequestId);
        }

        [Fact]
        public void RequestId_CanBeNull()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Act
            model.RequestId = null;

            // Assert
            Assert.Null(model.RequestId);
        }

        [Fact]
        public void ShowRequestId_ReturnsFalse_WhenRequestIdIsNull()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = null };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShowRequestId_ReturnsFalse_WhenRequestIdIsEmpty()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = string.Empty };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShowRequestId_ReturnsFalse_WhenRequestIdIsWhitespace()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "   " };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShowRequestId_ReturnsTrue_WhenRequestIdHasValue()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "test-request-id" };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShowRequestId_ReturnsTrue_WhenRequestIdIsNotEmpty()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "123" };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData("test-id", true)]
        [InlineData("12345", true)]
        public void ShowRequestId_ReturnsExpectedValue_ForVariousInputs(string? requestId, bool expected)
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = requestId };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ErrorViewModel_CanBeInitializedWithObjectInitializer()
        {
            // Arrange & Act
            var model = new ErrorViewModel
            {
                RequestId = "test-id"
            };

            // Assert
            Assert.Equal("test-id", model.RequestId);
            Assert.True(model.ShowRequestId);
        }
    }
}
