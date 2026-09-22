using Microsoft.Extensions.Logging;
using Moq;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Services;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Tests.Application;

/// <summary>
/// Unit tests for <see cref="UserProfileService"/> using mocked dependencies.
/// </summary>
public class UserProfileServiceTests
{
    private readonly Mock<IUserProfileRepository> _repositoryMock;
    private readonly Mock<ILogger<UserProfileService>> _loggerMock;
    private readonly UserProfileService _service;

    public UserProfileServiceTests()
    {
        _repositoryMock = new Mock<IUserProfileRepository>();
        _loggerMock = new Mock<ILogger<UserProfileService>>();
        _service = new UserProfileService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDto_WhenEntityExists()
    {
        // Arrange
        var entity = new UserProfile
        {
            Id = 1,
            IdentityUserId = "identity-001",
            DisplayName = "Alice",
            Email = "alice@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Alice", result.DisplayName);
        Assert.Equal("alice@example.com", result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityNotFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_ShouldReturnDto_WhenEntityExists()
    {
        // Arrange
        var entity = new UserProfile
        {
            Id = 2,
            IdentityUserId = "identity-002",
            DisplayName = "Bob",
            Email = "bob@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _repositoryMock
            .Setup(r => r.GetByIdentityUserIdAsync("identity-002", default))
            .ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdentityUserIdAsync("identity-002");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Bob", result.DisplayName);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDtos()
    {
        // Arrange
        var entities = new List<UserProfile>
        {
            new() { Id = 1, IdentityUserId = "u1", DisplayName = "User 1", Email = "u1@test.com", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, IdentityUserId = "u2", DisplayName = "User 2", Email = "u2@test.com", CreatedAt = DateTime.UtcNow }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(entities.AsReadOnly());

        // Act
        var results = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.DisplayName == "User 1");
        Assert.Contains(results, r => r.DisplayName == "User 2");
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryAddAndReturnDto()
    {
        // Arrange
        var dto = new UserProfileDto
        {
            IdentityUserId = "identity-003",
            DisplayName = "Charlie",
            Email = "charlie@example.com",
            IsActive = true
        };
        var createdEntity = new UserProfile
        {
            Id = 3,
            IdentityUserId = dto.IdentityUserId,
            DisplayName = dto.DisplayName,
            Email = dto.Email,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserProfile>(), default))
            .ReturnsAsync(createdEntity);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("Charlie", result.DisplayName);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallRepositoryUpdate()
    {
        // Arrange
        var dto = new UserProfileDto
        {
            Id = 4,
            IdentityUserId = "identity-004",
            DisplayName = "Updated User",
            Email = "updated@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<UserProfile>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(dto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserProfile>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        _repositoryMock.Setup(r => r.DeleteAsync(5, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(5);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(5, default), Times.Once);
    }
}
