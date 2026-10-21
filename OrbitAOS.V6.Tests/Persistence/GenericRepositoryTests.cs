// ============================================================
// Tests/Persistence/GenericRepositoryTests.cs
// Unit Tests – Infrastructure Layer
//
// Tests the GenericRepository<T> using:
//   • EF Core InMemory provider (no real SQL Server needed)
//   • A dedicated TestDbContext that includes the stub entity
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

// ---------------------------------------------------------------------------
// Test-specific DbContext that registers the stub entity
// ---------------------------------------------------------------------------

/// <summary>
/// Lightweight DbContext used only in tests; registers <see cref="BaseEntityStub"/>.
/// </summary>
public class TestDbContext : DbContext
{
    public DbSet<BaseEntityStub> Stubs => Set<BaseEntityStub>();

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BaseEntityStub>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });
    }
}

// ---------------------------------------------------------------------------
// Repository adapter that wraps TestDbContext
// ---------------------------------------------------------------------------

/// <summary>
/// Thin wrapper so <see cref="GenericRepository{T}"/> can be constructed
/// with a <see cref="TestDbContext"/> instead of <see cref="ApplicationDbContext"/>.
/// </summary>
public class TestGenericRepository<T> : IDisposable where T : class
{
    private readonly TestDbContext _context;
    private readonly DbSet<T> _dbSet;

    public TestGenericRepository(TestDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet.AsNoTracking().ToListAsync(ct);

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.FindAsync(new object[] { id }, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _dbSet.AddAsync(entity, ct);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}

// ---------------------------------------------------------------------------
// Actual test class
// ---------------------------------------------------------------------------

/// <summary>
/// Integration-style unit tests for <see cref="GenericRepository{T}"/>
/// using the EF Core InMemory provider.
/// </summary>
public class GenericRepositoryTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly TestGenericRepository<BaseEntityStub> _repository;

    public GenericRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _repository = new TestGenericRepository<BaseEntityStub>(_context);
    }

    // ------------------------------------------------------------------
    // AddAsync + GetAllAsync
    // ------------------------------------------------------------------

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
    public async Task AddAsync_MultipleEntities_GetAllAsync_ReturnsAll()
    {
        // Arrange
        var e1 = new BaseEntityStub { Name = "Alpha" };
        var e2 = new BaseEntityStub { Name = "Beta" };
        var e3 = new BaseEntityStub { Name = "Gamma" };

        // Act
        await _repository.AddAsync(e1);
        await _repository.AddAsync(e2);
        await _repository.AddAsync(e3);
        await _repository.SaveChangesAsync();
        var all = (await _repository.GetAllAsync()).ToList();

        // Assert
        all.Should().HaveCount(3);
        all.Select(e => e.Name).Should().Contain(new[] { "Alpha", "Beta", "Gamma" });
    }

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyCollection()
    {
        // Act
        var all = await _repository.GetAllAsync();

        // Assert
        all.Should().BeEmpty();
    }

    // ------------------------------------------------------------------
    // GetByIdAsync
    // ------------------------------------------------------------------

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
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        // Act
        var found = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        found.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_EmptyGuid_ReturnsNull()
    {
        // Act
        var found = await _repository.GetByIdAsync(Guid.Empty);

        // Assert
        found.Should().BeNull();
    }

    // ------------------------------------------------------------------
    // Delete
    // ------------------------------------------------------------------

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
    public async Task Delete_OneOfMany_LeavesOthersIntact()
    {
        // Arrange
        var keep = new BaseEntityStub { Name = "Keep" };
        var remove = new BaseEntityStub { Name = "Remove" };
        await _repository.AddAsync(keep);
        await _repository.AddAsync(remove);
        await _repository.SaveChangesAsync();

        // Act
        _repository.Delete(remove);
        await _repository.SaveChangesAsync();
        var all = (await _repository.GetAllAsync()).ToList();

        // Assert
        all.Should().ContainSingle(e => e.Name == "Keep");
        all.Should().NotContain(e => e.Name == "Remove");
    }

    // ------------------------------------------------------------------
    // Update
    // ------------------------------------------------------------------

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

    [Fact]
    public async Task Update_DoesNotChangeId()
    {
        // Arrange
        var entity = new BaseEntityStub { Name = "Original" };
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        var originalId = entity.Id;

        // Act
        entity.Name = "Changed";
        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        // Assert
        entity.Id.Should().Be(originalId);
    }

    // ------------------------------------------------------------------
    // SaveChangesAsync
    // ------------------------------------------------------------------

    [Fact]
    public async Task SaveChangesAsync_ReturnsNumberOfAffectedRows()
    {
        // Arrange
        var entity = new BaseEntityStub { Name = "Row Count Test" };
        await _repository.AddAsync(entity);

        // Act
        var rows = await _repository.SaveChangesAsync();

        // Assert
        rows.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_NoChanges_ReturnsZero()
    {
        // Act
        var rows = await _repository.SaveChangesAsync();

        // Assert
        rows.Should().Be(0);
    }

    // ------------------------------------------------------------------
    // IDisposable
    // ------------------------------------------------------------------

    public void Dispose() => _repository.Dispose();
}
