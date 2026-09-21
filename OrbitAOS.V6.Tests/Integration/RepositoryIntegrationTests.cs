using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Integration;

/// <summary>
/// Integration tests for the generic Repository using EF Core InMemory database.
/// Tests actual data access operations without mocking the database layer.
/// Uses a unique in-memory database per test to ensure test isolation.
/// </summary>
public class RepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<SampleEntity> _repository;

    public RepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<SampleEntity>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var entity = new SampleEntity
        {
            Name = "Test Sample",
            Description = "Test Description",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        // Act
        await _repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.SampleEntities
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Name == "Test Sample");
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Sample");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        // Arrange
        var entity = new SampleEntity
        {
            Name = "Test Sample",
            Description = "Test Description",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        _context.SampleEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(entity.Id);
        result.Name.Should().Be("Test Sample");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNonDeletedEntities()
    {
        // Arrange
        var entities = new List<SampleEntity>
        {
            new SampleEntity { Name = "Sample 1", Description = "Description 1", IsActive = true, CreatedDate = DateTime.UtcNow },
            new SampleEntity { Name = "Sample 2", Description = "Description 2", IsActive = true, CreatedDate = DateTime.UtcNow }
        };
        _context.SampleEntities.AddRange(entities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldExcludeSoftDeletedEntities()
    {
        // Arrange
        var activeEntity = new SampleEntity { Name = "Active", Description = "Active", IsActive = true, CreatedDate = DateTime.UtcNow, IsDeleted = false };
        var deletedEntity = new SampleEntity { Name = "Deleted", Description = "Deleted", IsActive = false, CreatedDate = DateTime.UtcNow, IsDeleted = true };
        _context.SampleEntities.AddRange(activeEntity, deletedEntity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Active");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var entity = new SampleEntity
        {
            Name = "Original Name",
            Description = "Original Description",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        _context.SampleEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Name = "Updated Name";
        await _repository.UpdateAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.SampleEntities
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == entity.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var entity = new SampleEntity
        {
            Name = "To Delete",
            Description = "Will be deleted",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        _context.SampleEntities.Add(entity);
        await _context.SaveChangesAsync();
        var entityId = entity.Id;

        // Act
        await _repository.DeleteAsync(entityId);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.SampleEntities
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == entityId);
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityExists_ShouldReturnTrue()
    {
        // Arrange
        var entity = new SampleEntity
        {
            Name = "Exists Sample",
            Description = "Description",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        _context.SampleEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(entity.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityNotExists_ShouldReturnFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(99999);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
