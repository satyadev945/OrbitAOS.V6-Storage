using OrbitAOS.Domain.Entities;
using Xunit;

namespace OrbitAOS.Domain.Entities
{
    /// <summary>
    /// Comprehensive unit tests for <see cref="UserProfile"/>.
    /// </summary>
    public class UserProfileTests
    {
        // ── Constructor / Default State Tests ───────────────────────────────

        [Fact]
        public void UserProfile_DefaultConstructor_CreatesInstance()
        {
            // Act
            var profile = new UserProfile();

            // Assert
            Assert.NotNull(profile);
        }

        [Fact]
        public void UserProfile_DefaultId_IsZero()
        {
            // Act
            var profile = new UserProfile();

            // Assert
            Assert.Equal(0, profile.Id);
        }

        [Fact]
        public void UserProfile_DefaultIdentityUserId_IsEmptyString()
        {
            // Act
            var profile = new UserProfile();

            // Assert
            Assert.Equal(string.Empty, profile.IdentityUserId);
        }

        [Fact]
        public void UserProfile_DefaultDisplayName_IsEmptyString()
        {
            // Act
            var profile = new UserProfile();

            // Assert
            Assert.Equal(string.Empty, profile.DisplayName);
        }

        [Fact]
        public void UserProfile_DefaultEmail_IsEmptyString()
        {
            // Act
            var profile = new UserProfile();

            // Assert
            Assert.Equal(string.Empty, profile.Email);
        }

        [Fact]
        public void UserProfile_DefaultIsActive_IsTrue()
        {
            // Act
            var profile = new UserProfile();

            // Assert
            Assert.True(profile.IsActive);
        }

        [Fact]
        public void UserProfile_DefaultUpdatedAt_IsNull()
        {
            // Act
            var profile = new UserProfile();

            // Assert
            Assert.Null(profile.UpdatedAt);
        }

        [Fact]
        public void UserProfile_DefaultCreatedAt_IsApproximatelyUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var profile = new UserProfile();
            var after = DateTime.UtcNow;

            // Assert
            Assert.True(profile.CreatedAt >= before && profile.CreatedAt <= after);
        }

        // ── Property Setter Tests ────────────────────────────────────────────

        [Fact]
        public void IdentityUserId_SetValue_ReturnsValue()
        {
            // Arrange
            var profile = new UserProfile();

            // Act
            profile.IdentityUserId = "identity-xyz";

            // Assert
            Assert.Equal("identity-xyz", profile.IdentityUserId);
        }

        [Fact]
        public void DisplayName_SetValue_ReturnsValue()
        {
            // Arrange
            var profile = new UserProfile();

            // Act
            profile.DisplayName = "Jane Smith";

            // Assert
            Assert.Equal("Jane Smith", profile.DisplayName);
        }

        [Fact]
        public void Email_SetValue_ReturnsValue()
        {
            // Arrange
            var profile = new UserProfile();

            // Act
            profile.Email = "jane@example.com";

            // Assert
            Assert.Equal("jane@example.com", profile.Email);
        }

        [Fact]
        public void IsActive_SetFalse_ReturnsFalse()
        {
            // Arrange
            var profile = new UserProfile();

            // Act
            profile.IsActive = false;

            // Assert
            Assert.False(profile.IsActive);
        }

        [Fact]
        public void IsActive_SetTrue_ReturnsTrue()
        {
            // Arrange
            var profile = new UserProfile { IsActive = false };

            // Act
            profile.IsActive = true;

            // Assert
            Assert.True(profile.IsActive);
        }

        [Fact]
        public void Id_SetValue_ReturnsValue()
        {
            // Arrange
            var profile = new UserProfile();

            // Act
            profile.Id = 77;

            // Assert
            Assert.Equal(77, profile.Id);
        }

        [Fact]
        public void CreatedAt_SetValue_ReturnsValue()
        {
            // Arrange
            var profile = new UserProfile();
            var date = new DateTime(2024, 2, 14, 9, 0, 0, DateTimeKind.Utc);

            // Act
            profile.CreatedAt = date;

            // Assert
            Assert.Equal(date, profile.CreatedAt);
        }

        [Fact]
        public void UpdatedAt_SetValue_ReturnsValue()
        {
            // Arrange
            var profile = new UserProfile();
            var date = new DateTime(2024, 9, 1, 16, 0, 0, DateTimeKind.Utc);

            // Act
            profile.UpdatedAt = date;

            // Assert
            Assert.Equal(date, profile.UpdatedAt);
        }

        [Fact]
        public void UpdatedAt_SetNull_ReturnsNull()
        {
            // Arrange
            var profile = new UserProfile { UpdatedAt = DateTime.UtcNow };

            // Act
            profile.UpdatedAt = null;

            // Assert
            Assert.Null(profile.UpdatedAt);
        }

        // ── Inheritance Tests ────────────────────────────────────────────────

        [Fact]
        public void UserProfile_InheritsFromBaseEntity()
        {
            // Act
            var profile = new UserProfile();

            // Assert
            Assert.IsAssignableFrom<OrbitAOS.Domain.Common.BaseEntity>(profile);
        }

        // ── Object Initializer Tests ─────────────────────────────────────────

        [Fact]
        public void UserProfile_ObjectInitializer_SetsAllProperties()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var updatedAt = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            // Act
            var profile = new UserProfile
            {
                Id = 55,
                IdentityUserId = "init-identity",
                DisplayName = "Init Profile",
                Email = "init@profile.com",
                IsActive = false,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            Assert.Equal(55, profile.Id);
            Assert.Equal("init-identity", profile.IdentityUserId);
            Assert.Equal("Init Profile", profile.DisplayName);
            Assert.Equal("init@profile.com", profile.Email);
            Assert.False(profile.IsActive);
            Assert.Equal(createdAt, profile.CreatedAt);
            Assert.Equal(updatedAt, profile.UpdatedAt);
        }

        [Theory]
        [InlineData(1, "uid-1", "Alice", "alice@example.com", true)]
        [InlineData(2, "uid-2", "Bob", "bob@example.com", false)]
        [InlineData(3, "uid-3", "Charlie", "charlie@example.com", true)]
        public void UserProfile_Theory_VariousValues(int id, string identityUserId, string displayName, string email, bool isActive)
        {
            // Arrange & Act
            var profile = new UserProfile
            {
                Id = id,
                IdentityUserId = identityUserId,
                DisplayName = displayName,
                Email = email,
                IsActive = isActive
            };

            // Assert
            Assert.Equal(id, profile.Id);
            Assert.Equal(identityUserId, profile.IdentityUserId);
            Assert.Equal(displayName, profile.DisplayName);
            Assert.Equal(email, profile.Email);
            Assert.Equal(isActive, profile.IsActive);
        }
    }
}
