// ============================================================
// Tests/Persistence/GenericRepositoryTests.cs
// Unit Tests – Infrastructure Layer
//
// Tests the GenericRepository<T> using:
//   • EF Core InMemory provider (no real SQL Server needed)
//   • xUnit as the test framework
//   • FluentAssertions for readable assertions
// ============================================================

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Persistence;
using Xunit;

namespace OrbitAOS.V6.Tests.Persistence;

/// <summary>
/// Integration-style unit tests for <see cref="GenericRepository{T}"/>
/// using the EF Core InMemory provider.
/// </summary>
public class GenericRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GenericRepository<BaseEntityStub> _repository;

    public GenericRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new GenericRepository<BaseEntityStub>(_context);
    }

    [Fact]
    public async Task AddAsync_And_GetAllAsync_ReturnAddedEntity()
    {
        // Arrange
        var entity = new BaseEntityStub { Name = "Test Entity" };

        // Act
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        var all = await _repository.GetAllAsync();

        // Assert
        all.Should().ContainSingle(e => e.Name == "Test Entity");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectEntity()
    {
        // Arrange
        var entity = new BaseEntityStub { Name = "Find Me" };
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        // Act
        var found = await _repository.GetByIdAsync(entity.Id);

        // Assert
        found.Should().NotBeNull();
        found!.Name.Should().Be("Find Me");
    }

    [Fact]
    public async Task Delete_RemovesEntity()
    {
        // Arrange
        var entity = new BaseEntityStub { Name = "Delete Me" };
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        // Act
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
        var all = await _repository.GetAllAsync();

        // Assert
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task Update_PersistsChanges()
    {
        // Arrange
        var entity = new BaseEntityStub { Name = "Original" };
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        // Act
        entity.Name = "Updated";
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
        var found = await _repository.GetByIdAsync(entity.Id);

        // Assert
        found!.Name.Should().Be("Updated");
    }

    public void Dispose() => _context.Dispose();
}

// ---------------------------------------------------------------------------
// Stub entity used only in tests (not a real domain entity)
// ---------------------------------------------------------------------------

/// <summary>
/// Minimal concrete entity for testing <see cref="GenericRepository{T}"/>.
/// </summary>
public class BaseEntityStub : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
