// ============================================================
// Tests/Data/RepositoryTests.cs
// Unit tests for the generic Repository<T> (Infrastructure layer)
// Uses EF Core InMemory provider for fast, isolated tests.
// ============================================================
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Repositories;
using FluentAssertions;

namespace OrbitAOS.V6.Tests.Data;

/// <summary>
/// Test entity for repository tests.
/// Uses a concrete entity that inherits BaseEntity.
/// </summary>
public class TestEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Unit tests for the generic Repository using EF Core InMemory.
/// Demonstrates the Repository pattern replacing direct DbContext
/// usage in legacy ASP.NET MVC controllers.
/// </summary>
public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<TestEntity> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<TestEntity>(_context);
    }

    [Fact]
    public async Task AddAsync_AddsEntityToDatabase()
    {
        // Arrange
        var entity = new TestEntity { Name = "Test Entity 1" };

        // Act
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var count = await _context.Set<TestEntity>().CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        _context.Set<TestEntity>().AddRange(
            new TestEntity { Name = "Entity A" },
            new TestEntity { Name = "Entity B" },
            new TestEntity { Name = "Entity C" }
        );
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        results.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectEntity()
    {
        // Arrange
        var entity = new TestEntity { Name = "Find Me" };
        _context.Set<TestEntity>().Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repository.GetByIdAsync(entity.Id);

        // Assert
        found.Should().NotBeNull();
        found!.Name.Should().Be("Find Me");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenEntityExists()
    {
        // Arrange
        _context.Set<TestEntity>().Add(new TestEntity { Name = "Existing" });
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(e => e.Name == "Existing");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenEntityDoesNotExist()
    {
        // Act
        var exists = await _repository.ExistsAsync(e => e.Name == "NonExistent");

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task Remove_DeletesEntityFromDatabase()
    {
        // Arrange
        var entity = new TestEntity { Name = "To Delete" };
        _context.Set<TestEntity>().Add(entity);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(entity);
        await _context.SaveChangesAsync();

        // Assert
        var count = await _context.Set<TestEntity>().CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task Update_ModifiesEntityInDatabase()
    {
        // Arrange
        var entity = new TestEntity { Name = "Original Name" };
        _context.Set<TestEntity>().Add(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Name = "Updated Name";
        _repository.Update(entity);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.Set<TestEntity>().FindAsync(entity.Id);
        updated!.Name.Should().Be("Updated Name");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
