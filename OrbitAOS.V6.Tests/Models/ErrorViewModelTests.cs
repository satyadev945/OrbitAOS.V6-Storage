using OrbitAOS.V6.Models;
using Xunit;

namespace OrbitAOS.V6.Tests.Models;

/// <summary>
/// Comprehensive unit tests for ErrorViewModel.
/// Covers all properties and computed members.
/// </summary>
public class ErrorViewModelTests
{
    // ─── Constructor / Instantiation Tests ──────────────────────────────────

    [Fact]
    public void ErrorViewModel_CanBeInstantiated()
    {
        // Act
        var model = new ErrorViewModel();

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void ErrorViewModel_DefaultRequestId_IsNull()
    {
        // Arrange & Act
        var model = new ErrorViewModel();

        // Assert
        Assert.Null(model.RequestId);
    }

    [Fact]
    public void ErrorViewModel_DefaultShowRequestId_IsFalse()
    {
        // Arrange & Act
        var model = new ErrorViewModel();

        // Assert
        Assert.False(model.ShowRequestId);
    }

    // ─── RequestId Property Tests ────────────────────────────────────────────

    [Fact]
    public void RequestId_SetToNonEmptyString_ReturnsCorrectValue()
    {
        // Arrange
        var model = new ErrorViewModel();

        // Act
        model.RequestId = "abc-123";

        // Assert
        Assert.Equal("abc-123", model.RequestId);
    }

    [Fact]
    public void RequestId_SetToNull_ReturnsNull()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "initial" };

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

    [Fact]
    public void RequestId_SetToGuidString_ReturnsGuidString()
    {
        // Arrange
        var model = new ErrorViewModel();
        var guid = Guid.NewGuid().ToString();

        // Act
        model.RequestId = guid;

        // Assert
        Assert.Equal(guid, model.RequestId);
    }

    [Fact]
    public void RequestId_SetToLongString_ReturnsLongString()
    {
        // Arrange
        var model = new ErrorViewModel();
        var longString = new string('x', 1000);

        // Act
        model.RequestId = longString;

        // Assert
        Assert.Equal(longString, model.RequestId);
    }

    // ─── ShowRequestId Computed Property Tests ───────────────────────────────

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
    public void ShowRequestId_WhenRequestIdIsNonEmpty_ReturnsTrue()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "some-request-id" };

        // Assert
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_WhenRequestIdIsWhitespace_ReturnsTrue()
    {
        // Arrange: whitespace is NOT empty per string.IsNullOrEmpty
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
    public void ShowRequestId_WhenRequestIdIsSingleChar_ReturnsTrue()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "a" };

        // Assert
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_ChangesWhenRequestIdChanges()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = null };
        Assert.False(model.ShowRequestId);

        // Act
        model.RequestId = "new-id";

        // Assert
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_ChangesFromTrueToFalse_WhenRequestIdSetToNull()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "some-id" };
        Assert.True(model.ShowRequestId);

        // Act
        model.RequestId = null;

        // Assert
        Assert.False(model.ShowRequestId);
    }

    [Fact]
    public void ShowRequestId_ChangesFromTrueToFalse_WhenRequestIdSetToEmpty()
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = "some-id" };
        Assert.True(model.ShowRequestId);

        // Act
        model.RequestId = string.Empty;

        // Assert
        Assert.False(model.ShowRequestId);
    }

    // ─── Object Initializer Tests ────────────────────────────────────────────

    [Fact]
    public void ErrorViewModel_ObjectInitializer_SetsRequestId()
    {
        // Arrange & Act
        var model = new ErrorViewModel { RequestId = "init-id" };

        // Assert
        Assert.Equal("init-id", model.RequestId);
        Assert.True(model.ShowRequestId);
    }

    [Fact]
    public void ErrorViewModel_IsPublicClass()
    {
        // Assert
        Assert.True(typeof(ErrorViewModel).IsPublic);
    }

    [Fact]
    public void ErrorViewModel_RequestId_IsNullableString()
    {
        // Arrange
        var prop = typeof(ErrorViewModel).GetProperty(nameof(ErrorViewModel.RequestId));

        // Assert
        Assert.NotNull(prop);
        Assert.Equal(typeof(string), prop!.PropertyType);
    }

    [Fact]
    public void ErrorViewModel_ShowRequestId_IsReadOnlyBool()
    {
        // Arrange
        var prop = typeof(ErrorViewModel).GetProperty(nameof(ErrorViewModel.ShowRequestId));

        // Assert
        Assert.NotNull(prop);
        Assert.Equal(typeof(bool), prop!.PropertyType);
        Assert.False(prop.CanWrite); // computed property, no setter
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("abc", true)]
    [InlineData("00000000-0000-0000-0000-000000000000", true)]
    [InlineData("trace-id-xyz", true)]
    public void ShowRequestId_Theory_VariousRequestIds(string? requestId, bool expectedShow)
    {
        // Arrange
        var model = new ErrorViewModel { RequestId = requestId };

        // Assert
        Assert.Equal(expectedShow, model.ShowRequestId);
    }
}
