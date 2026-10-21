// ============================================================
// Tests/Infrastructure/Identity/ApplicationUserTests.cs
// Unit tests for ApplicationUser — xUnit
// ============================================================

using OrbitAOS.V6.Infrastructure.Identity;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure.Identity;

public class ApplicationUserTests
{
    // ── Default Values ───────────────────────────────────────────────────────

    [Fact]
    public void ApplicationUser_DefaultDisplayName_IsNull()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        Assert.Null(user.DisplayName);
    }

    [Fact]
    public void ApplicationUser_DefaultLastLoginAt_IsNull()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        Assert.Null(user.LastLoginAt);
    }

    [Fact]
    public void ApplicationUser_DefaultIsActive_IsTrue()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void ApplicationUser_DefaultCreatedAt_IsRecentUtc()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var user = new ApplicationUser();

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.True(user.CreatedAt >= before && user.CreatedAt <= after,
            $"CreatedAt {user.CreatedAt} should be between {before} and {after}");
    }

    // ── Property Setters ─────────────────────────────────────────────────────

    [Fact]
    public void ApplicationUser_SetDisplayName_Persists()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.DisplayName = "John Doe";

        // Assert
        Assert.Equal("John Doe", user.DisplayName);
    }

    [Fact]
    public void ApplicationUser_SetLastLoginAt_Persists()
    {
        // Arrange
        var user = new ApplicationUser();
        var loginTime = new DateTime(2024, 3, 15, 9, 0, 0, DateTimeKind.Utc);

        // Act
        user.LastLoginAt = loginTime;

        // Assert
        Assert.Equal(loginTime, user.LastLoginAt);
    }

    [Fact]
    public void ApplicationUser_SetIsActive_ToFalse_Persists()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.IsActive = false;

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void ApplicationUser_SetCreatedAt_Persists()
    {
        // Arrange
        var user = new ApplicationUser();
        var customDate = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        user.CreatedAt = customDate;

        // Assert
        Assert.Equal(customDate, user.CreatedAt);
    }

    [Fact]
    public void ApplicationUser_InheritsIdentityUser_UserNameIsNull()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert — IdentityUser.UserName defaults to null
        Assert.Null(user.UserName);
    }

    [Fact]
    public void ApplicationUser_SetUserName_Persists()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.UserName = "jdoe@example.com";

        // Assert
        Assert.Equal("jdoe@example.com", user.UserName);
    }

    [Fact]
    public void ApplicationUser_SetEmail_Persists()
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.Email = "jdoe@example.com";

        // Assert
        Assert.Equal("jdoe@example.com", user.Email);
    }

    [Fact]
    public void ApplicationUser_FullyPopulated_AllPropertiesCorrect()
    {
        // Arrange
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var lastLogin = new DateTime(2024, 6, 1, 8, 0, 0, DateTimeKind.Utc);

        // Act
        var user = new ApplicationUser
        {
            UserName = "admin@orbitaos.com",
            Email = "admin@orbitaos.com",
            DisplayName = "Admin User",
            LastLoginAt = lastLogin,
            IsActive = true,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal("admin@orbitaos.com", user.UserName);
        Assert.Equal("admin@orbitaos.com", user.Email);
        Assert.Equal("Admin User", user.DisplayName);
        Assert.Equal(lastLogin, user.LastLoginAt);
        Assert.True(user.IsActive);
        Assert.Equal(createdAt, user.CreatedAt);
    }

    [Theory]
    [InlineData("Alice")]
    [InlineData("Bob Smith")]
    [InlineData("")]
    public void ApplicationUser_SetDisplayName_VariousValues(string name)
    {
        // Arrange
        var user = new ApplicationUser();

        // Act
        user.DisplayName = name;

        // Assert
        Assert.Equal(name, user.DisplayName);
    }
}
