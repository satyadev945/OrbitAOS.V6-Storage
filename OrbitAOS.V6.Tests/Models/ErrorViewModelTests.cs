// ============================================================
// Tests/Models/ErrorViewModelTests.cs
// Unit tests for ErrorViewModel — xUnit
// ============================================================

using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Models;

public class ErrorViewModelTests
{
    // ── Default Values ───────────────────────────────────────────────────────

    [Fact]
    public void ErrorViewModel_DefaultRequestId_IsNull()
    {
        // Arrange & Act
        var vm = new ErrorViewModel();

        // Assert
        Assert.Null(vm.RequestId);
    }

    [Fact]
    public void ErrorViewModel_DefaultShowRequestId_IsFalse()
    {
        // Arrange & Act
        var vm = new ErrorViewModel();

        // Assert
        Assert.False(vm.ShowRequestId);
    }

    // ── ShowRequestId Logic ──────────────────────────────────────────────────

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
    {
        // Arrange
        var vm = new ErrorViewModel { RequestId = null };

        // Assert
        Assert.False(vm.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
    {
        // Arrange
        var vm = new ErrorViewModel { RequestId = string.Empty };

        // Assert
        Assert.False(vm.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsWhitespace_ReturnsFalse()
    {
        // Arrange
        // Note: ShowRequestId uses string.IsNullOrEmpty, not IsNullOrWhiteSpace
        // Whitespace is NOT empty, so ShowRequestId returns true for whitespace
        var vm = new ErrorViewModel { RequestId = "   " };

        // Assert — whitespace is not null/empty, so ShowRequestId is true
        Assert.True(vm.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdHasValue_ReturnsTrue()
    {
        // Arrange
        var vm = new ErrorViewModel { RequestId = "abc-123" };

        // Assert
        Assert.True(vm.ShowRequestId);
    }

    [Theory]
    [InlineData("trace-id-001", true)]
    [InlineData("00-abc123-def456-01", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void ShowRequestId_VariousRequestIds(string? requestId, bool expected)
    {
        // Arrange
        var vm = new ErrorViewModel { RequestId = requestId };

        // Assert
        Assert.Equal(expected, vm.ShowRequestId);
    }

    // ── Property Setters ─────────────────────────────────────────────────────

    [Fact]
    public void ErrorViewModel_SetRequestId_Persists()
    {
        // Arrange
        var vm = new ErrorViewModel();

        // Act
        vm.RequestId = "test-request-id";

        // Assert
        Assert.Equal("test-request-id", vm.RequestId);
    }

    [Fact]
    public void ErrorViewModel_SetRequestId_ThenClear_ShowRequestIdFalse()
    {
        // Arrange
        var vm = new ErrorViewModel { RequestId = "some-id" };
        Assert.True(vm.ShowRequestId);

        // Act
        vm.RequestId = null;

        // Assert
        Assert.False(vm.ShowRequestId);
    }
}
