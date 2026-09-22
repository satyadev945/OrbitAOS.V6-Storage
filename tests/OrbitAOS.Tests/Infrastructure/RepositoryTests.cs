using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Services;

namespace OrbitAOS.Tests.Infrastructure;

/// <summary>
/// Unit tests for the generic <see cref="Repository{T}"/> using an in-memory database.
/// </summary>
public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<UserProfile> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<UserProfile>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistEntity()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "user-001",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true
        };

        // Act
        var result = await _repository.AddAsync(profile);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Test User", result.DisplayName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "user-002",
            DisplayName = "Find Me",
            Email = "findme@example.com",
            IsActive = true
        };
        await _repository.AddAsync(profile);

        // Act
        var result = await _repository.GetByIdAsync(profile.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Find Me", result.DisplayName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        await _repository.AddAsync(new UserProfile { IdentityUserId = "u1", DisplayName = "User 1", Email = "u1@test.com" });
        await _repository.AddAsync(new UserProfile { IdentityUserId = "u2", DisplayName = "User 2", Email = "u2@test.com" });

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyEntity()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "user-003",
            DisplayName = "Original Name",
            Email = "original@example.com",
            IsActive = true
        };
        await _repository.AddAsync(profile);

        // Act
        profile.DisplayName = "Updated Name";
        await _repository.UpdateAsync(profile);
        var updated = await _repository.GetByIdAsync(profile.Id);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.DisplayName);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "user-004",
            DisplayName = "To Delete",
            Email = "delete@example.com",
            IsActive = true
        };
        await _repository.AddAsync(profile);
        var id = profile.Id;

        // Act
        await _repository.DeleteAsync(id);
        var result = await _repository.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
