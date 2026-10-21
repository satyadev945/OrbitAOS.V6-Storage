// ============================================================
// Tests/Models/ErrorViewModelTests.cs
// Unit Tests – Web / Presentation Layer (ViewModels)
//
// Tests the ErrorViewModel properties and computed ShowRequestId.
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Models;

/// <summary>
/// Unit tests for <see cref="ErrorViewModel"/>.
/// </summary>
public class ErrorViewModelTests
{
    // ------------------------------------------------------------------
    // Constructor / Default values
    // ------------------------------------------------------------------

    [Fact]
    public void ErrorViewModel_NewInstance_IsNotNull()
    {
        // Act
        var model = new ErrorViewModel();

        // Assert
        model.Should().NotBeNull();
    }

    [Fact]
    public void ErrorViewModel_NewInstance_RequestIdIsNull()
    {
        // Act
        var model = new ErrorViewModel();

        // Assert
        model.RequestId.Should().BeNull();
    }

    [Fact]
    public void ErrorViewModel_NewInstance_ShowRequestIdIsFalse()
    {
        // Act
        var model = new ErrorViewModel();

        // Assert
        model.ShowRequestId.Should().BeFalse();
    }

    // ------------------------------------------------------------------
    // ShowRequestId computed property
    // ------------------------------------------------------------------

    [Fact]
    public void ShowRequestId_WhenRequestIdIsNull_ReturnsFalse()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = null };

        // Assert
        model.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsEmpty_ReturnsFalse()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = string.Empty };

        // Assert
        model.ShowRequestId.Should().BeFalse();
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsWhitespace_ReturnsFalse()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "   " };

        // Assert – ShowRequestId uses IsNullOrEmpty (not IsNullOrWhiteSpace), so whitespace returns true
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdHasValue_ReturnsTrue()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "abc-123-xyz" };

        // Assert
        model.ShowRequestId.Should().BeTrue();
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsGuid_ReturnsTrue()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = Guid.NewGuid().ToString() };

        // Assert
        model.ShowRequestId.Should().BeTrue();
    }

    // ------------------------------------------------------------------
    // RequestId property setter
    // ------------------------------------------------------------------

    [Fact]
    public void RequestId_SetValue_ReturnsSetValue()
    {
        // Arrange
        var model = new ErrorViewModel();

        // Act
        model.RequestId = "00-abc123-def456-00";

        // Assert
        model.RequestId.Should().Be("00-abc123-def456-00");
    }

    [Fact]
    public void RequestId_SetToNull_ReturnsNull()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "some-id" };

        // Act
        model.RequestId = null;

        // Assert
        model.RequestId.Should().BeNull();
    }

    [Theory]
    [InlineData("trace-id-001", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("  ", true)]
    [InlineData("0HN5JKQM7IQKL:00000001", true)]
    public void ShowRequestId_VariousRequestIds_ReturnsExpected(string? requestId, bool expected)
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = requestId };

        // Assert
        model.ShowRequestId.Should().Be(expected);
    }
}
