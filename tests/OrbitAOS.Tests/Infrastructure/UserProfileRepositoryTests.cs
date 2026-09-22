using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Identity;

namespace OrbitAOS.Tests.Infrastructure;

/// <summary>
/// Unit tests for <see cref="UserProfileRepository"/> using an in-memory database.
/// </summary>
public class UserProfileRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserProfileRepository _repository;

    public UserProfileRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new UserProfileRepository(_context);
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_ShouldReturnProfile_WhenExists()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-abc",
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true
        };
        await _repository.AddAsync(profile);

        // Act
        var result = await _repository.GetByIdentityUserIdAsync("identity-abc");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test User", result.DisplayName);
        Assert.Equal("identity-abc", result.IdentityUserId);
    }

    [Fact]
    public async Task GetByIdentityUserIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdentityUserIdAsync("non-existent-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAssignId()
    {
        // Arrange
        var profile = new UserProfile
        {
            IdentityUserId = "identity-xyz",
            DisplayName = "New User",
            Email = "new@example.com",
            IsActive = true
        };

        // Act
        var result = await _repository.AddAsync(profile);

        // Assert
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoProfiles()
    {
        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(results);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
