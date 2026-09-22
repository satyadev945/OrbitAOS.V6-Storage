using OrbitAOS.Domain.Common;
using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Tests.Domain;

/// <summary>
/// Unit tests for domain entities.
/// </summary>
public class DomainEntityTests
{
    [Fact]
    public void UserProfile_ShouldHaveDefaultValues()
    {
        // Act
        var profile = new UserProfile();

        // Assert
        Assert.Equal(string.Empty, profile.IdentityUserId);
        Assert.Equal(string.Empty, profile.DisplayName);
        Assert.Equal(string.Empty, profile.Email);
        Assert.True(profile.IsActive);
    }

    [Fact]
    public void UserProfile_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var profile = new UserProfile();

        // Assert
        Assert.IsAssignableFrom<BaseEntity>(profile);
    }

    [Fact]
    public void BaseEntity_CreatedAt_ShouldDefaultToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var profile = new UserProfile();
        var after = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(profile.CreatedAt >= before && profile.CreatedAt <= after);
    }

    [Fact]
    public void BaseEntity_UpdatedAt_ShouldDefaultToNull()
    {
        // Act
        var profile = new UserProfile();

        // Assert
        Assert.Null(profile.UpdatedAt);
    }

    [Fact]
    public void UserProfile_ShouldSetProperties()
    {
        // Arrange & Act
        var profile = new UserProfile
        {
            Id = 42,
            IdentityUserId = "identity-123",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = false
        };

        // Assert
        Assert.Equal(42, profile.Id);
        Assert.Equal("identity-123", profile.IdentityUserId);
        Assert.Equal("Test User", profile.DisplayName);
        Assert.Equal("test@example.com", profile.Email);
        Assert.False(profile.IsActive);
    }
}
