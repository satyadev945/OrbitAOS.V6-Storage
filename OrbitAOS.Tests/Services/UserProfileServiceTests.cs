using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="UserProfileService"/>.
    /// </summary>
    public class UserProfileServiceTests
    {
        private readonly Mock<IUserProfileRepository> _repositoryMock;
        private readonly UserProfileService _service;

        /// <summary>
        /// Initializes test fixtures for UserProfileService tests.
        /// </summary>
        public UserProfileServiceTests()
        {
            _repositoryMock = new Mock<IUserProfileRepository>();
            _service = new UserProfileService(_repositoryMock.Object);
        }

        /// <summary>
        /// Verifies that GetByIdAsync returns a mapped DTO when the entity exists.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenEntityExists()
        {
            // Arrange
            var entity = new UserProfile
            {
                Id = 1,
                IdentityUserId = "user-id-1",
                DisplayName = "Test User",
                Email = "test@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test User", result.DisplayName);
            Assert.Equal("test@example.com", result.Email);
        }

        /// <summary>
        /// Verifies that GetByIdAsync returns null when the entity does not exist.
        /// </summary>
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

        /// <summary>
        /// Verifies that GetAllAsync returns a mapped collection of DTOs.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            // Arrange
            var entities = new List<UserProfile>
            {
                new() { Id = 1, IdentityUserId = "uid-1", DisplayName = "User One", Email = "one@example.com", IsActive = true, CreatedAt = DateTime.UtcNow },
                new() { Id = 2, IdentityUserId = "uid-2", DisplayName = "User Two", Email = "two@example.com", IsActive = true, CreatedAt = DateTime.UtcNow }
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

            // Act
            var result = (await _service.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("User One", result[0].DisplayName);
            Assert.Equal("User Two", result[1].DisplayName);
        }

        /// <summary>
        /// Verifies that CreateAsync calls AddAsync on the repository and returns the created DTO.
        /// </summary>
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
                .ReturnsAsync((UserProfile entity) =>
                {
                    entity.Id = 10;
                    return entity;
                });

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("New User", result.DisplayName);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserProfile>()), Times.Once);
        }

        /// <summary>
        /// Verifies that UpdateAsync calls UpdateAsync on the repository.
        /// </summary>
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

        /// <summary>
        /// Verifies that DeleteAsync calls DeleteAsync on the repository.
        /// </summary>
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

        /// <summary>
        /// Verifies that GetByIdentityUserIdAsync returns a mapped DTO when the entity exists.
        /// </summary>
        [Fact]
        public async Task GetByIdentityUserIdAsync_ReturnsDto_WhenEntityExists()
        {
            // Arrange
            var entity = new UserProfile
            {
                Id = 7,
                IdentityUserId = "identity-abc",
                DisplayName = "Identity User",
                Email = "identity@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _repositoryMock.Setup(r => r.GetByIdentityUserIdAsync("identity-abc")).ReturnsAsync(entity);

            // Act
            var result = await _service.GetByIdentityUserIdAsync("identity-abc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.Id);
            Assert.Equal("identity-abc", result.IdentityUserId);
        }
    }
}
