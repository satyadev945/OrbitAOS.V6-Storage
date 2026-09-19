using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Application.Common.Models;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.Tests.Services
{
    /// <summary>
    /// Unit tests for UserProfileService.
    /// Demonstrates service layer testing with mocked repositories.
    /// </summary>
    public class UserProfileServiceTests
    {
        private readonly Mock<IRepository<UserProfile>> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<UserProfileService>> _mockLogger;
        private readonly UserProfileService _service;

        public UserProfileServiceTests()
        {
            _mockRepository = new Mock<IRepository<UserProfile>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<UserProfileService>>();
            _service = new UserProfileService(
                _mockRepository.Object,
                _mockUnitOfWork.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProfileExists_ReturnsSuccessResult()
        {
            // Arrange
            var profile = new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-123",
                DisplayName = "Test User",
                Department = "Engineering",
                JobTitle = "Developer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1, default))
                .ReturnsAsync(profile);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Test User", result.Data.DisplayName);
            Assert.Equal("user-123", result.Data.IdentityUserId);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProfileNotFound_ReturnsFailureResult()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999, default))
                .ReturnsAsync((UserProfile?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("999", result.ErrorMessage);
        }

        [Fact]
        public async Task CreateAsync_WithValidDto_ReturnsSuccessResult()
        {
            // Arrange
            var dto = new CreateUserProfileDto(
                "user-456",
                "New User",
                "Marketing",
                "Manager"
            );

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserProfile>(), default))
                .ReturnsAsync((UserProfile p, CancellationToken _) => p);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("New User", result.Data.DisplayName);
            Assert.Equal("user-456", result.Data.IdentityUserId);
        }

        [Fact]
        public async Task UpdateAsync_WhenProfileExists_ReturnsSuccessResult()
        {
            // Arrange
            var profile = new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-123",
                DisplayName = "Old Name",
                IsActive = true
            };
            var dto = new UpdateUserProfileDto("New Name", "IT", "Senior Dev", true);

            _mockRepository.Setup(r => r.GetByIdAsync(1, default))
                .ReturnsAsync(profile);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task UpdateAsync_WhenProfileNotFound_ReturnsFailureResult()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999, default))
                .ReturnsAsync((UserProfile?)null);
            var dto = new UpdateUserProfileDto("Name", null, null, true);

            // Act
            var result = await _service.UpdateAsync(999, dto);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task DeleteAsync_WhenProfileExists_ReturnsSuccessResult()
        {
            // Arrange
            var profile = new UserProfile { Id = 1, IdentityUserId = "user-123", DisplayName = "User" };
            _mockRepository.Setup(r => r.GetByIdAsync(1, default))
                .ReturnsAsync(profile);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
                .ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProfiles()
        {
            // Arrange
            var profiles = new List<UserProfile>
            {
                new UserProfile { Id = 1, IdentityUserId = "u1", DisplayName = "User One", IsActive = true },
                new UserProfile { Id = 2, IdentityUserId = "u2", DisplayName = "User Two", IsActive = true }
            };
            _mockRepository.Setup(r => r.GetAllAsync(default))
                .ReturnsAsync(profiles);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());
        }
    }
}
