// ============================================================
// Tests/Domain/ApplicationUserTests.cs
// Unit Tests – Domain Layer
//
// Tests the ApplicationUser entity properties and defaults.
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain;

/// <summary>
/// Unit tests for <see cref="ApplicationUser"/>.
/// </summary>
public class ApplicationUserTests
{
    // ------------------------------------------------------------------
    // Constructor / Default values
    // ------------------------------------------------------------------

    [Fact]
    public void ApplicationUser_NewInstance_IsNotNull()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.Should().NotBeNull();
    }

    [Fact]
    public void ApplicationUser_NewInstance_IsActiveByDefault()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void ApplicationUser_NewInstance_CreatedAtIsSetToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var user = new ApplicationUser();

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        user.CreatedAt.Should().BeAfter(before).And.BeBefore(after);
    }

    [Fact]
    public void ApplicationUser_NewInstance_DisplayNameIsNull()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.DisplayName.Should().BeNull();
    }

    [Fact]
    public void ApplicationUser_NewInstance_UpdatedAtIsNull()
    {
        // Act
        var user = new ApplicationUser();

        // Assert
        user.UpdatedAt.Should().BeNull();
    }

    // ------------------------------------------------------------------
    // Property setters
    // ------------------------------------------------------------------

    [Fact]
    public void ApplicationUser_SetDisplayName_ReturnsSetValue()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.DisplayName = "John Doe";

        // Assert
        user.DisplayName.Should().Be("John Doe");
    }

    [Fact]
    public void ApplicationUser_SetDisplayNameToNull_ReturnsNull()
    {
        // Arrange
        var user = new ApplicationUser { DisplayName = "Jane" };

        // Act
        user.DisplayName = null;

        // Assert
        user.DisplayName.Should().BeNull();
    }

    [Fact]
    public void ApplicationUser_SetDisplayNameToEmpty_ReturnsEmpty()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.DisplayName = string.Empty;

        // Assert
        user.DisplayName.Should().BeEmpty();
    }

    [Fact]
    public void ApplicationUser_SetIsActiveToFalse_ReturnsFalse()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.IsActive = false;

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void ApplicationUser_SetUpdatedAt_ReturnsSetValue()
    {
        // Arrange
        var user = new ApplicationUser();
        var timestamp = new DateTime(2024, 3, 10, 8, 0, 0, DateTimeKind.Utc);

        // Act
        user.UpdatedAt = timestamp;

        // Assert
        user.UpdatedAt.Should().Be(timestamp);
    }

    [Fact]
    public void ApplicationUser_SetCreatedAt_ReturnsSetValue()
    {
        // Arrange
        var user = new ApplicationUser();
        var timestamp = new DateTime(2023, 12, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        user.CreatedAt = timestamp;

        // Assert
        user.CreatedAt.Should().Be(timestamp);
    }

    // ------------------------------------------------------------------
    // IdentityUser inherited properties
    // ------------------------------------------------------------------

    [Fact]
    public void ApplicationUser_SetUserName_ReturnsSetValue()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.UserName = "testuser@example.com";

        // Assert
        user.UserName.Should().Be("testuser@example.com");
    }

    [Fact]
    public void ApplicationUser_SetEmail_ReturnsSetValue()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.Email = "test@example.com";

        // Assert
        user.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void ApplicationUser_TwoInstances_AreIndependent()
    {
        // Act
        var user1 = new ApplicationUser { DisplayName = "Alice" };
        var user2 = new ApplicationUser { DisplayName = "Bob" };

        // Assert
        user1.DisplayName.Should().NotBe(user2.DisplayName);
    }

    [Fact]
    public void ApplicationUser_SetDisplayNameWithSpecialChars_ReturnsSetValue()
    {
        // Arrange
        var user = new ApplicationUser();
        const string specialName = "José García-López";

        // Act
        user.DisplayName = specialName;

        // Assert
        user.DisplayName.Should().Be(specialName);
    }
}
