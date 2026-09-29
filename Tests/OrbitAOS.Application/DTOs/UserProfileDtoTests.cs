using OrbitAOS.Application.DTOs;
using Xunit;

namespace OrbitAOS.Application.DTOs
{
    /// <summary>
    /// Comprehensive unit tests for <see cref="UserProfileDto"/>.
    /// </summary>
    public class UserProfileDtoTests
    {
        // ── Constructor / Default State Tests ───────────────────────────────

        [Fact]
        public void UserProfileDto_DefaultConstructor_CreatesInstance()
        {
            // Act
            var dto = new UserProfileDto();

            // Assert
            Assert.NotNull(dto);
        }

        [Fact]
        public void UserProfileDto_DefaultId_IsZero()
        {
            // Act
            var dto = new UserProfileDto();

            // Assert
            Assert.Equal(0, dto.Id);
        }

        [Fact]
        public void UserProfileDto_DefaultIdentityUserId_IsEmptyString()
        {
            // Act
            var dto = new UserProfileDto();

            // Assert
            Assert.Equal(string.Empty, dto.IdentityUserId);
        }

        [Fact]
        public void UserProfileDto_DefaultDisplayName_IsEmptyString()
        {
            // Act
            var dto = new UserProfileDto();

            // Assert
            Assert.Equal(string.Empty, dto.DisplayName);
        }

        [Fact]
        public void UserProfileDto_DefaultEmail_IsEmptyString()
        {
            // Act
            var dto = new UserProfileDto();

            // Assert
            Assert.Equal(string.Empty, dto.Email);
        }

        [Fact]
        public void UserProfileDto_DefaultIsActive_IsFalse()
        {
            // Act
            var dto = new UserProfileDto();

            // Assert
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void UserProfileDto_DefaultUpdatedAt_IsNull()
        {
            // Act
            var dto = new UserProfileDto();

            // Assert
            Assert.Null(dto.UpdatedAt);
        }

        // ── Property Setter Tests ────────────────────────────────────────────

        [Fact]
        public void Id_SetValue_ReturnsValue()
        {
            // Arrange
            var dto = new UserProfileDto();

            // Act
            dto.Id = 42;

            // Assert
            Assert.Equal(42, dto.Id);
        }

        [Fact]
        public void IdentityUserId_SetValue_ReturnsValue()
        {
            // Arrange
            var dto = new UserProfileDto();

            // Act
            dto.IdentityUserId = "user-abc-123";

            // Assert
            Assert.Equal("user-abc-123", dto.IdentityUserId);
        }

        [Fact]
        public void DisplayName_SetValue_ReturnsValue()
        {
            // Arrange
            var dto = new UserProfileDto();

            // Act
            dto.DisplayName = "John Doe";

            // Assert
            Assert.Equal("John Doe", dto.DisplayName);
        }

        [Fact]
        public void Email_SetValue_ReturnsValue()
        {
            // Arrange
            var dto = new UserProfileDto();

            // Act
            dto.Email = "john@example.com";

            // Assert
            Assert.Equal("john@example.com", dto.Email);
        }

        [Fact]
        public void IsActive_SetTrue_ReturnsTrue()
        {
            // Arrange
            var dto = new UserProfileDto();

            // Act
            dto.IsActive = true;

            // Assert
            Assert.True(dto.IsActive);
        }

        [Fact]
        public void IsActive_SetFalse_ReturnsFalse()
        {
            // Arrange
            var dto = new UserProfileDto { IsActive = true };

            // Act
            dto.IsActive = false;

            // Assert
            Assert.False(dto.IsActive);
        }

        [Fact]
        public void CreatedAt_SetValue_ReturnsValue()
        {
            // Arrange
            var dto = new UserProfileDto();
            var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            dto.CreatedAt = date;

            // Assert
            Assert.Equal(date, dto.CreatedAt);
        }

        [Fact]
        public void UpdatedAt_SetValue_ReturnsValue()
        {
            // Arrange
            var dto = new UserProfileDto();
            var date = new DateTime(2024, 6, 15, 12, 30, 0, DateTimeKind.Utc);

            // Act
            dto.UpdatedAt = date;

            // Assert
            Assert.Equal(date, dto.UpdatedAt);
        }

        [Fact]
        public void UpdatedAt_SetNull_ReturnsNull()
        {
            // Arrange
            var dto = new UserProfileDto { UpdatedAt = DateTime.UtcNow };

            // Act
            dto.UpdatedAt = null;

            // Assert
            Assert.Null(dto.UpdatedAt);
        }

        // ── Object Initializer Tests ─────────────────────────────────────────

        [Fact]
        public void UserProfileDto_ObjectInitializer_SetsAllProperties()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var updatedAt = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var dto = new UserProfileDto
            {
                Id = 99,
                IdentityUserId = "init-uid",
                DisplayName = "Init User",
                Email = "init@example.com",
                IsActive = true,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            Assert.Equal(99, dto.Id);
            Assert.Equal("init-uid", dto.IdentityUserId);
            Assert.Equal("Init User", dto.DisplayName);
            Assert.Equal("init@example.com", dto.Email);
            Assert.True(dto.IsActive);
            Assert.Equal(createdAt, dto.CreatedAt);
            Assert.Equal(updatedAt, dto.UpdatedAt);
        }

        [Theory]
        [InlineData(1, "uid-1", "User A", "a@example.com", true)]
        [InlineData(2, "uid-2", "User B", "b@example.com", false)]
        [InlineData(100, "uid-100", "User C", "c@example.com", true)]
        public void UserProfileDto_Theory_VariousValues(int id, string identityUserId, string displayName, string email, bool isActive)
        {
            // Arrange & Act
            var dto = new UserProfileDto
            {
                Id = id,
                IdentityUserId = identityUserId,
                DisplayName = displayName,
                Email = email,
                IsActive = isActive
            };

            // Assert
            Assert.Equal(id, dto.Id);
            Assert.Equal(identityUserId, dto.IdentityUserId);
            Assert.Equal(displayName, dto.DisplayName);
            Assert.Equal(email, dto.Email);
            Assert.Equal(isActive, dto.IsActive);
        }
    }
}
