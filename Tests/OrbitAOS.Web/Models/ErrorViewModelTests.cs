using OrbitAOS.Web.Models;
using Xunit;

namespace OrbitAOS.Web.Models
{
    /// <summary>
    /// Comprehensive unit tests for <see cref="ErrorViewModel"/>.
    /// </summary>
    public class ErrorViewModelTests
    {
        // ── Constructor / Default State Tests ───────────────────────────────

        [Fact]
        public void ErrorViewModel_DefaultConstructor_CreatesInstance()
        {
            // Act
            var model = new ErrorViewModel();

            // Assert
            Assert.NotNull(model);
        }

        [Fact]
        public void ErrorViewModel_DefaultRequestId_IsNull()
        {
            // Act
            var model = new ErrorViewModel();

            // Assert
            Assert.Null(model.RequestId);
        }

        [Fact]
        public void ErrorViewModel_DefaultShowRequestId_IsFalse()
        {
            // Act
            var model = new ErrorViewModel();

            // Assert
            Assert.False(model.ShowRequestId);
        }

        // ── RequestId Property Tests ─────────────────────────────────────────

        [Fact]
        public void RequestId_SetToValue_ReturnsValue()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Act
            model.RequestId = "req-001";

            // Assert
            Assert.Equal("req-001", model.RequestId);
        }

        [Fact]
        public void RequestId_SetToNull_ReturnsNull()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "some-id" };

            // Act
            model.RequestId = null;

            // Assert
            Assert.Null(model.RequestId);
        }

        [Fact]
        public void RequestId_SetToEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Act
            model.RequestId = string.Empty;

            // Assert
            Assert.Equal(string.Empty, model.RequestId);
        }

        [Fact]
        public void RequestId_SetToWhitespace_ReturnsWhitespace()
        {
            // Arrange
            var model = new ErrorViewModel();

            // Act
            model.RequestId = "   ";

            // Assert
            Assert.Equal("   ", model.RequestId);
        }

        // ── ShowRequestId Property Tests ─────────────────────────────────────

        [Fact]
        public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = null };

            // Assert
            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = string.Empty };

            // Assert
            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsSet_ReturnsTrue()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "test-request-id" };

            // Assert
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsWhitespace_ReturnsTrue()
        {
            // Arrange - whitespace is NOT empty per IsNullOrEmpty
            var model = new ErrorViewModel { RequestId = "   " };

            // Assert
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsGuid_ReturnsTrue()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = Guid.NewGuid().ToString() };

            // Assert
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsActivityId_ReturnsTrue()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01" };

            // Assert
            Assert.True(model.ShowRequestId);
        }

        // ── Object Initializer Tests ─────────────────────────────────────────

        [Fact]
        public void ErrorViewModel_ObjectInitializer_SetsAllProperties()
        {
            // Arrange & Act
            var model = new ErrorViewModel
            {
                RequestId = "init-id-999"
            };

            // Assert
            Assert.Equal("init-id-999", model.RequestId);
            Assert.True(model.ShowRequestId);
        }

        [Theory]
        [InlineData("req-1", true)]
        [InlineData("req-2", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void ShowRequestId_Theory_VariousRequestIds(string? requestId, bool expected)
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = requestId };

            // Assert
            Assert.Equal(expected, model.ShowRequestId);
        }
    }
}
