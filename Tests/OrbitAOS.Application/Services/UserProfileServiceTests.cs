using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.Application.Services
{
    /// <summary>
    /// Comprehensive unit tests for <see cref="UserProfileService"/>.
    /// </summary>
    public class UserProfileServiceTests
    {
        private readonly Mock<IUserProfileRepository> _repositoryMock;
        private readonly UserProfileService _service;

        public UserProfileServiceTests()
        {
            _repositoryMock = new Mock<IUserProfileRepository>();
            _service = new UserProfileService(_repositoryMock.Object);
        }

        // ── Constructor Tests ────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidRepository_CreatesInstance()
        {
            // Arrange & Act
            var service = new UserProfileService(_repositoryMock.Object);

            // Assert
            Assert.NotNull(service);
        }

        // ── GetByIdAsync Tests ───────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenEntityExists()
        {
            // Arrange
            var entity = CreateSampleUserProfile(1, "uid-1", "Test User", "test@example.com");
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("uid-1", result.IdentityUserId);
            Assert.Equal("Test User", result.DisplayName);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenEntityNotFound()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_MapsAllFields_Correctly()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
            var updatedAt = new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);
            var entity = new UserProfile
            {
                Id = 5,
                IdentityUserId = "identity-5",
                DisplayName = "Full Map User",
                Email = "fullmap@example.com",
                IsActive = false,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
            _repositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Id);
            Assert.Equal("identity-5", result.IdentityUserId);
            Assert.Equal("Full Map User", result.DisplayName);
            Assert.Equal("fullmap@example.com", result.Email);
            Assert.False(result.IsActive);
            Assert.Equal(createdAt, result.CreatedAt);
            Assert.Equal(updatedAt, result.UpdatedAt);
        }

        [Fact]
        public async Task GetByIdAsync_WithZeroId_CallsRepository()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(0)).ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _service.GetByIdAsync(0);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetByIdAsync(0), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithNegativeId_CallsRepository()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(-1)).ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _service.GetByIdAsync(-1);

            // Assert
            Assert.Null(result);
        }

        // ── GetByIdentityUserIdAsync Tests ───────────────────────────────────

        [Fact]
        public async Task GetByIdentityUserIdAsync_ReturnsDto_WhenEntityExists()
        {
            // Arrange
            var entity = CreateSampleUserProfile(7, "identity-abc", "Identity User", "identity@example.com");
            _repositoryMock.Setup(r => r.GetByIdentityUserIdAsync("identity-abc")).ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdentityUserIdAsync("identity-abc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.Id);
            Assert.Equal("identity-abc", result.IdentityUserId);
        }

        [Fact]
        public async Task GetByIdentityUserIdAsync_ReturnsNull_WhenEntityNotFound()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdentityUserIdAsync("nonexistent")).ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _service.GetByIdentityUserIdAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdentityUserIdAsync_WithEmptyString_CallsRepository()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdentityUserIdAsync(string.Empty)).ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _service.GetByIdentityUserIdAsync(string.Empty);

            // Assert
            Assert.Null(result);
            _repositoryMock.Verify(r => r.GetByIdentityUserIdAsync(string.Empty), Times.Once);
        }

        // ── GetAllAsync Tests ────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            // Arrange
            var entities = new List<UserProfile>
            {
                CreateSampleUserProfile(1, "uid-1", "User One", "one@example.com"),
                CreateSampleUserProfile(2, "uid-2", "User Two", "two@example.com")
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Act
            var result = (await _service.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("User One", result[0].DisplayName);
            Assert.Equal("User Two", result[1].DisplayName);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyCollection_WhenNoEntities()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserProfile>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSingleItem_WhenOneEntityExists()
        {
            // Arrange
            var entities = new List<UserProfile>
            {
                CreateSampleUserProfile(1, "uid-1", "Solo User", "solo@example.com")
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Act
            var result = (await _service.GetAllAsync()).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("Solo User", result[0].DisplayName);
        }

        [Fact]
        public async Task GetAllAsync_MapsAllFieldsCorrectly()
        {
            // Arrange
            var createdAt = new DateTime(2024, 3, 10, 8, 0, 0, DateTimeKind.Utc);
            var entities = new List<UserProfile>
            {
                new()
                {
                    Id = 10,
                    IdentityUserId = "uid-10",
                    DisplayName = "Mapped User",
                    Email = "mapped@example.com",
                    IsActive = true,
                    CreatedAt = createdAt,
                    UpdatedAt = null
                }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Act
            var result = (await _service.GetAllAsync()).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(10, result[0].Id);
            Assert.Equal("uid-10", result[0].IdentityUserId);
            Assert.Equal("Mapped User", result[0].DisplayName);
            Assert.Equal("mapped@example.com", result[0].Email);
            Assert.True(result[0].IsActive);
            Assert.Equal(createdAt, result[0].CreatedAt);
            Assert.Null(result[0].UpdatedAt);
        }

        // ── CreateAsync Tests ────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_CallsRepositoryAddAsync_AndReturnsDto()
        {
            // Arrange
            var dto = new UserProfileDto
            {
                IdentityUserId = "new-user-id",
                DisplayName = "New User",
                Email = "new@example.com",
                IsActive = true
            };
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserProfile>()))
                .ReturnsAsync((UserProfile entity) => { entity.Id = 10; return entity; });

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("New User", result.DisplayName);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserProfile>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_SetsCreatedAtToUtcNow()
        {
            // Arrange
            var dto = new UserProfileDto
            {
                IdentityUserId = "uid-new",
                DisplayName = "Timestamp User",
                Email = "ts@example.com",
                IsActive = true
            };
            var before = DateTime.UtcNow;
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserProfile>()))
                .ReturnsAsync((UserProfile entity) => entity);

            // Act
            var result = await _service.CreateAsync(dto);
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.CreatedAt >= before && result.CreatedAt <= after);
        }

        [Fact]
        public async Task CreateAsync_MapsAllDtoFieldsToEntity()
        {
            // Arrange
            UserProfile? capturedEntity = null;
            var dto = new UserProfileDto
            {
                IdentityUserId = "map-uid",
                DisplayName = "Map User",
                Email = "map@example.com",
                IsActive = false
            };
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<UserProfile>()))
                .Callback<UserProfile>(e => capturedEntity = e)
                .ReturnsAsync((UserProfile entity) => entity);

            // Act
            await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(capturedEntity);
            Assert.Equal("map-uid", capturedEntity.IdentityUserId);
            Assert.Equal("Map User", capturedEntity.DisplayName);
            Assert.Equal("map@example.com", capturedEntity.Email);
            Assert.False(capturedEntity.IsActive);
        }

        // ── UpdateAsync Tests ────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_CallsRepositoryUpdateAsync()
        {
            // Arrange
            var dto = new UserProfileDto
            {
                Id = 5,
                IdentityUserId = "uid-5",
                DisplayName = "Updated User",
                Email = "updated@example.com",
                IsActive = true
            };
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UserProfile>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserProfile>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SetsUpdatedAtToUtcNow()
        {
            // Arrange
            UserProfile? capturedEntity = null;
            var dto = new UserProfileDto { Id = 3, IdentityUserId = "uid-3", DisplayName = "U", Email = "u@e.com" };
            var before = DateTime.UtcNow;
            _repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<UserProfile>()))
                .Callback<UserProfile>(e => capturedEntity = e)
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(capturedEntity);
            Assert.NotNull(capturedEntity.UpdatedAt);
            Assert.True(capturedEntity.UpdatedAt >= before && capturedEntity.UpdatedAt <= after);
        }

        [Fact]
        public async Task UpdateAsync_MapsAllDtoFieldsToEntity()
        {
            // Arrange
            UserProfile? capturedEntity = null;
            var dto = new UserProfileDto
            {
                Id = 8,
                IdentityUserId = "uid-8",
                DisplayName = "Updated Display",
                Email = "upd@example.com",
                IsActive = false
            };
            _repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<UserProfile>()))
                .Callback<UserProfile>(e => capturedEntity = e)
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            Assert.NotNull(capturedEntity);
            Assert.Equal(8, capturedEntity.Id);
            Assert.Equal("uid-8", capturedEntity.IdentityUserId);
            Assert.Equal("Updated Display", capturedEntity.DisplayName);
            Assert.Equal("upd@example.com", capturedEntity.Email);
            Assert.False(capturedEntity.IsActive);
        }

        // ── DeleteAsync Tests ────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_CallsRepositoryDeleteAsync()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync(3)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(3);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(3), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithZeroId_CallsRepository()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync(0)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(0);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(0), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithLargeId_CallsRepository()
        {
            // Arrange
            _repositoryMock.Setup(r => r.DeleteAsync(int.MaxValue)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(int.MaxValue);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(int.MaxValue), Times.Once);
        }

        // ── Helper Methods ───────────────────────────────────────────────────

        private static UserProfile CreateSampleUserProfile(int id, string identityUserId, string displayName, string email)
            => new()
            {
                Id = id,
                IdentityUserId = identityUserId,
                DisplayName = displayName,
                Email = email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
    }
}
