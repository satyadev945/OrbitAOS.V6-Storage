// ============================================================
// Infrastructure/Persistence/UnitOfWork.cs
// Clean Architecture – Infrastructure Layer
//
// Wraps the EF Core DbContext to implement the Unit-of-Work
// pattern.  Ensures all repository operations in a single
// request share one transaction.
// ============================================================

using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;

namespace OrbitAOS.V6.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private bool _disposed;

    /// <summary>
    /// Initialises a new instance of <see cref="UnitOfWork"/>.
    /// </summary>
    /// <param name="context">The EF Core database context.</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        // EF Core does not require an explicit rollback – simply discard the context.
        // For explicit transaction support, use _context.Database.BeginTransactionAsync().
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
    }
}
