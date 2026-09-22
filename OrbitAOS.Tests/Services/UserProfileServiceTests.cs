using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using Xunit;

namespace OrbitAOS.Tests.Services;

/// <summary>
/// Unit tests for <see cref="UserProfileService"/>.
/// </summary>
public class UserProfileServiceTests
{
    private readonly Mock<IUserProfileRepository> _repositoryMock;
    private readonly IUserProfileService _service;

    /// <summary>Initializes test fixtures.</summary>
    public UserProfileServiceTests()
    {
        _repositoryMock = new Mock<IUserProfileRepository>();
        _service = new UserProfileService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsMappedDto()
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
        _repositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test User", result.DisplayName);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityNotFound_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllMappedDtos()
    {
        // Arrange
        var entities = new List<UserProfile>
        {
            new() { Id = 1, IdentityUserId = "uid1", DisplayName = "User One", Email = "one@example.com", IsActive = true, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, IdentityUserId = "uid2", DisplayName = "User Two", Email = "two@example.com", IsActive = false, CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(entities);

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("User One", result[0].DisplayName);
        Assert.Equal("User Two", result[1].DisplayName);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryAddAndReturnsDto()
    {
        // Arrange
        var dto = new UserProfileDto
        {
            IdentityUserId = "new-uid",
            DisplayName = "New User",
            Email = "new@example.com",
            IsActive = true
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserProfile>(), default))
            .ReturnsAsync((UserProfile entity, CancellationToken _) =>
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
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(5, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(5);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(5, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_WhenFound_ReturnsMappedDto()
    {
        // Arrange
        var entity = new UserProfile
        {
            Id = 3,
            IdentityUserId = "identity-abc",
            DisplayName = "Identity User",
            Email = "identity@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _repositoryMock
            .Setup(r => r.GetByIdentityUserIdAsync("identity-abc", default))
            .ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdentityUserIdAsync("identity-abc");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Identity User", result.DisplayName);
    }
}
