using FluentAssertions;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Models
{
    /// <summary>
    /// Unit tests for ErrorViewModel – Web layer model.
    /// Covers property assignment and computed ShowRequestId property.
    /// </summary>
    public class ErrorViewModelTests
    {
        // ─── Constructor / Default State ───────────────────────────────────────

        [Fact]
        public void ErrorViewModel_DefaultConstructor_RequestIdIsNull()
        {
            // Arrange & Act
            var model = new ErrorViewModel();

            // Assert
            model.RequestId.Should().BeNull();
        }

        [Fact]
        public void ErrorViewModel_DefaultConstructor_ShowRequestIdIsFalse()
        {
            // Arrange & Act
            var model = new ErrorViewModel();

            // Assert
            model.ShowRequestId.Should().BeFalse();
        }

        // ─── ShowRequestId computed property ───────────────────────────────────

        [Fact]
        public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = null };

            // Act & Assert
            model.ShowRequestId.Should().BeFalse();
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = string.Empty };

            // Act & Assert
            model.ShowRequestId.Should().BeFalse();
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdIsWhitespace_ReturnsTrue()
        {
            // Arrange
            // Note: ShowRequestId uses string.IsNullOrEmpty (not IsNullOrWhiteSpace),
            // so whitespace-only strings are considered non-empty and return true.
            var model = new ErrorViewModel { RequestId = "   " };

            // Act & Assert
            model.ShowRequestId.Should().BeTrue();
        }

        [Fact]
        public void ShowRequestId_WhenRequestIdHasValue_ReturnsTrue()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "abc-123" };

            // Act & Assert
            model.ShowRequestId.Should().BeTrue();
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000001")]
        [InlineData("trace-id-xyz")]
        [InlineData("1")]
        public void ShowRequestId_WithVariousNonEmptyValues_ReturnsTrue(string requestId)
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = requestId };

            // Act & Assert
            model.ShowRequestId.Should().BeTrue();
        }

        // ─── Property assignment ────────────────────────────────────────────────

        [Fact]
        public void RequestId_CanBeSetAndRetrieved()
        {
            // Arrange
            const string expected = "test-request-id";
            var model = new ErrorViewModel();

            // Act
            model.RequestId = expected;

            // Assert
            model.RequestId.Should().Be(expected);
        }

        [Fact]
        public void RequestId_CanBeReassigned()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "first" };

            // Act
            model.RequestId = "second";

            // Assert
            model.RequestId.Should().Be("second");
            model.ShowRequestId.Should().BeTrue();
        }

        [Fact]
        public void RequestId_CanBeSetToNull_AfterHavingValue()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "some-id" };

            // Act
            model.RequestId = null;

            // Assert
            model.RequestId.Should().BeNull();
            model.ShowRequestId.Should().BeFalse();
        }
    }
}
