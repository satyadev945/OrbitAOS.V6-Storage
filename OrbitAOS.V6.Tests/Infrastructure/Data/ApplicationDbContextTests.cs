// ============================================================
// Tests/Infrastructure/Data/ApplicationDbContextTests.cs
// Unit tests for ApplicationDbContext — xUnit + EF Core InMemory
// ============================================================

using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Data;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure.Data;

public class ApplicationDbContextTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        // Assert
        Assert.NotNull(_context);
    }

    // ── AuditLogs DbSet ──────────────────────────────────────────────────────

    [Fact]
    public void AuditLogs_DbSet_IsNotNull()
    {
        // Assert
        Assert.NotNull(_context.AuditLogs);
    }

    [Fact]
    public async Task AuditLogs_AddAndSave_PersistsEntity()
    {
        // Arrange
        var log = new AuditLog
        {
            UserId = "user-1",
            Action = "Login",
            EntityType = "User",
            Timestamp = DateTime.UtcNow
        };

        // Act
        await _context.AuditLogs.AddAsync(log);
        var saved = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, saved);
        Assert.True(log.Id > 0);
    }

    [Fact]
    public async Task AuditLogs_QueryAll_ReturnsAddedEntities()
    {
        // Arrange
        var log1 = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        var log2 = new AuditLog { UserId = "u2", Action = "Update", EntityType = "Order" };
        await _context.AuditLogs.AddRangeAsync(log1, log2);
        await _context.SaveChangesAsync();

        // Act
        var all = await _context.AuditLogs.ToListAsync();

        // Assert
        Assert.Equal(2, all.Count);
    }

    // ── SaveChangesAsync (audit timestamps) ──────────────────────────────────

    [Fact]
    public async Task SaveChangesAsync_OnModifiedEntity_SetsUpdatedAt()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        // Act — modify the entity
        log.Action = "Logout";
        _context.AuditLogs.Update(log);
        await _context.SaveChangesAsync();

        // Assert
        Assert.NotNull(log.UpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_OnNewEntity_DoesNotSetUpdatedAt()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };

        // Act
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        // Assert — UpdatedAt should remain null for new entities
        Assert.Null(log.UpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_WithCancellationToken_Succeeds()
    {
        // Arrange
        var log = new AuditLog { UserId = "u1", Action = "Login", EntityType = "User" };
        await _context.AuditLogs.AddAsync(log);
        using var cts = new CancellationTokenSource();

        // Act
        var result = await _context.SaveChangesAsync(cts.Token);

        // Assert
        Assert.Equal(1, result);
    }

    // ── Multiple Entities ────────────────────────────────────────────────────

    [Fact]
    public async Task AuditLogs_AddMultiple_AllPersisted()
    {
        // Arrange
        var logs = Enumerable.Range(1, 5).Select(i => new AuditLog
        {
            UserId = $"user-{i}",
            Action = "Login",
            EntityType = "User"
        }).ToList();

        // Act
        await _context.AuditLogs.AddRangeAsync(logs);
        var saved = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(5, saved);
        var count = await _context.AuditLogs.CountAsync();
        Assert.Equal(5, count);
    }
}
