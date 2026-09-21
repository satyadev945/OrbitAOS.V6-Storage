using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.Tests.Services
{
    /// <summary>
    /// Unit tests for UserProfileService.
    /// Tests all business logic methods using Moq for repository dependencies.
    /// </summary>
    public class UserProfileServiceTests
    {
        private readonly Mock<IUserProfileRepository> _mockRepository;
        private readonly UserProfileService _service;

        public UserProfileServiceTests()
        {
            _mockRepository = new Mock<IUserProfileRepository>();
            _service = new UserProfileService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetProfileByIdAsync_ReturnsDto_WhenProfileExists()
        {
            // Arrange
            var entity = new UserProfile
            {
                Id = 1,
                UserId = "user-1",
                DisplayName = "Test User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            // Act
            var result = await _service.GetProfileByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test User", result.DisplayName);
        }

        [Fact]
        public async Task GetProfileByIdAsync_ReturnsNull_WhenProfileNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _service.GetProfileByIdAsync(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProfileByUserIdAsync_ReturnsDto_WhenProfileExists()
        {
            // Arrange
            var entity = new UserProfile
            {
                Id = 1,
                UserId = "user-abc",
                DisplayName = "ABC User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _mockRepository.Setup(r => r.GetByUserIdAsync("user-abc")).ReturnsAsync(entity);

            // Act
            var result = await _service.GetProfileByUserIdAsync("user-abc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user-abc", result.UserId);
        }

        [Fact]
        public async Task GetAllProfilesAsync_ReturnsAllProfiles()
        {
            // Arrange
            var entities = new List<UserProfile>
            {
                new() { Id = 1, UserId = "u1", DisplayName = "User 1", CreatedAt = DateTime.UtcNow },
                new() { Id = 2, UserId = "u2", DisplayName = "User 2", CreatedAt = DateTime.UtcNow }
            };
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllProfilesAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateProfileAsync_ReturnsCreatedDto()
        {
            // Arrange
            var dto = new UserProfileDto
            {
                UserId = "new-user",
                DisplayName = "New User",
                IsActive = true
            };
            var entity = new UserProfile
            {
                Id = 10,
                UserId = "new-user",
                DisplayName = "New User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserProfile>())).ReturnsAsync(entity);

            // Act
            var result = await _service.CreateProfileAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("New User", result.DisplayName);
        }

        [Fact]
        public async Task UpdateProfileAsync_ReturnsUpdatedDto_WhenProfileExists()
        {
            // Arrange
            var existing = new UserProfile
            {
                Id = 1,
                UserId = "user-1",
                DisplayName = "Old Name",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var updateDto = new UserProfileDto
            {
                DisplayName = "New Name",
                IsActive = true
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserProfile>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateProfileAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Name", result.DisplayName);
        }

        [Fact]
        public async Task UpdateProfileAsync_ReturnsNull_WhenProfileNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _service.UpdateProfileAsync(99, new UserProfileDto());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteProfileAsync_ReturnsTrue_WhenProfileExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteProfileAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProfileAsync_ReturnsFalse_WhenProfileNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

            // Act
            var result = await _service.DeleteProfileAsync(99);

            // Assert
            Assert.False(result);
        }
    }
}
