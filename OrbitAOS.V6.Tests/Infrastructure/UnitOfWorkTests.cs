// ============================================================
// Tests/Infrastructure/UnitOfWorkTests.cs
// Unit Tests – Infrastructure Layer
//
// Tests the UnitOfWork using:
//   • EF Core InMemory provider (no real SQL Server needed)
//   • xUnit as the test framework
//   • FluentAssertions for readable assertions
// ============================================================

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Infrastructure.Persistence;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure;

/// <summary>
/// Unit tests for <see cref="UnitOfWork"/>.
/// </summary>
public class UnitOfWorkTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UnitOfWork _sut;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sut = new UnitOfWork(_context);
    }

    // ------------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------------

    [Fact]
    public void Constructor_WithValidContext_CreatesInstance()
    {
        // Assert
        _sut.Should().NotBeNull();
    }

    // ------------------------------------------------------------------
    // CommitAsync
    // ------------------------------------------------------------------

    [Fact]
    public async Task CommitAsync_WithNoChanges_ReturnsZero()
    {
        // Act
        var result = await _sut.CommitAsync();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task CommitAsync_WithCancellationTokenNone_Succeeds()
    {
        // Act
        var result = await _sut.CommitAsync(CancellationToken.None);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task CommitAsync_WithDefaultCancellationToken_Succeeds()
    {
        // Act
        var result = await _sut.CommitAsync(default);

        // Assert
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task CommitAsync_ReturnsTask_IsAwaitable()
    {
        // Act
        var task = _sut.CommitAsync();

        // Assert
        task.Should().NotBeNull();
        var result = await task;
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    // ------------------------------------------------------------------
    // RollbackAsync
    // ------------------------------------------------------------------

    [Fact]
    public async Task RollbackAsync_CompletesSuccessfully()
    {
        // Act
        var act = async () => await _sut.RollbackAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RollbackAsync_WithCancellationTokenNone_Succeeds()
    {
        // Act
        var act = async () => await _sut.RollbackAsync(CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RollbackAsync_ReturnsCompletedTask()
    {
        // Act
        var task = _sut.RollbackAsync();

        // Assert
        task.Should().NotBeNull();
        await task; // Should complete without exception
    }

    // ------------------------------------------------------------------
    // Dispose
    // ------------------------------------------------------------------

    [Fact]
    public void Dispose_CalledOnce_DoesNotThrow()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        var uow = new UnitOfWork(context);

        // Act
        var act = () => uow.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new ApplicationDbContext(options);
        var uow = new UnitOfWork(context);

        // Act
        var act = () =>
        {
            uow.Dispose();
            uow.Dispose(); // Second call should be safe (idempotent)
        };

        // Assert
        act.Should().NotThrow();
    }

    // ------------------------------------------------------------------
    // IDisposable
    // ------------------------------------------------------------------

    public void Dispose() => _sut.Dispose();
}
