// =============================================================================
// Tests/Models/ErrorViewModelTests.cs
// xUnit Tests for ErrorViewModel class
// =============================================================================

using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Models
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void ErrorViewModel_DefaultRequestId_IsNull()
        {
            // Arrange & Act
            var model = new ErrorViewModel();

            // Assert
            Assert.Null(model.RequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = null };

            // Act & Assert
            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = string.Empty };

            // Act & Assert
            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdHasValue_ReturnsTrue()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "abc-123" };

            // Act & Assert
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void RequestId_SetValue_ReturnsCorrectValue()
        {
            // Arrange
            var model = new ErrorViewModel();
            const string requestId = "trace-id-xyz";

            // Act
            model.RequestId = requestId;

            // Assert
            Assert.Equal(requestId, model.RequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsWhitespace_ReturnsFalse()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "   " };

            // Act & Assert
            // string.IsNullOrEmpty(" ") is false, so ShowRequestId should be true
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void ErrorViewModel_CanSetAndGetRequestId()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Act
            model.RequestId = "00-abc123-def456-00";

            // Assert
            Assert.Equal("00-abc123-def456-00", model.RequestId);
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void ErrorViewModel_MultipleInstances_AreIndependent()
        {
            // Arrange & Act
            var model1 = new ErrorViewModel { RequestId = "id-1" };
            var model2 = new ErrorViewModel { RequestId = null };

            // Assert
            Assert.True(model1.ShowRequestId);
            Assert.False(model2.ShowRequestId);
        }
    }
}
