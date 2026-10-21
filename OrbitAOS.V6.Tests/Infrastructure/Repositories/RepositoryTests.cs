// ============================================================
// Tests/Infrastructure/Repositories/RepositoryTests.cs
// Unit tests for Repository<T> using EF Core InMemory — xUnit
// ============================================================

using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<AuditLog> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<AuditLog>(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidContext_CreatesInstance()
    {
        // Assert
        Assert.NotNull(_repository);
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithEntities_ReturnsAllEntities()
    {
        // Arrange
        var log1 = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        var log2 = new AuditLog { UserId = "u2", Action = "Update", EntityType = "Order" };
        await _context.AuditLogs.AddRangeAsync(log1, log2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_Succeeds()
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        // Act
        var result = await _repository.GetAllAsync(cts.Token);

        // Assert
        Assert.NotNull(result);
    }

    // ── FindAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task FindAsync_WithMatchingPredicate_ReturnsMatchingEntities()
    {
        // Arrange
        var log1 = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        var log2 = new AuditLog { UserId = "u2", Action = "Update", EntityType = "Order" };
        await _context.AuditLogs.AddRangeAsync(log1, log2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(e => e.UserId == "u1");

        // Assert
        Assert.Single(result);
        Assert.Equal("u1", result.First().UserId);
    }

    [Fact]
    public async Task FindAsync_WithNoMatchingPredicate_ReturnsEmpty()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(e => e.UserId == "nonexistent");

        // Assert
        Assert.Empty(result);
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsEntity()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(log.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(log.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }

    // ── AddAsync ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task AddAsync_ValidEntity_AddsToDatabase()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Create", EntityType = "Product" };

        // Act
        await _repository.AddAsync(log);
        await _repository.SaveChangesAsync();

        // Assert
        var all = await _repository.GetAllAsync();
        Assert.Single(all);
    }

    [Fact]
    public async Task AddAsync_MultipleEntities_AllAdded()
    {
        // Arrange
        var log1 = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        var log2 = new AuditLog { UserId = "u2", Action = "Logout", EntityType = "User" };

        // Act
        await _repository.AddAsync(log1);
        await _repository.AddAsync(log2);
        await _repository.SaveChangesAsync();

        // Assert
        var all = await _repository.GetAllAsync();
        Assert.Equal(2, all.Count());
    }

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_ExistingEntity_UpdatesInDatabase()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        // Act
        log.Action = "Logout";
        _repository.Update(log);
        await _repository.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(log.Id);
        Assert.Equal("Logout", updated!.Action);
    }

    // ── Delete (Soft Delete) ─────────────────────────────────────────────────

    [Fact]
    public async Task Delete_ExistingEntity_SetsIsDeletedTrue()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        // Act
        _repository.Delete(log);
        await _repository.SaveChangesAsync();

        // Assert
        Assert.True(log.IsDeleted);
    }

    // ── SaveChangesAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ReturnsZero()
    {
        // Act
        var result = await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task SaveChangesAsync_WithAddedEntity_ReturnsOne()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        await _repository.AddAsync(log);

        // Act
        var result = await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result);
    }
}
