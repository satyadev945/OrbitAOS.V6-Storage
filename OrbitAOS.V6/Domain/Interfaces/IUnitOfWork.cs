// ============================================================
// Domain/Interfaces/IUnitOfWork.cs
// Unit of Work interface - Domain layer contract
// Coordinates multiple repository operations in a single
// database transaction. Replaces direct SaveChanges() calls
// scattered across legacy controllers.
// ============================================================
namespace OrbitAOS.V6.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface.
/// Ensures all repository operations within a business transaction
/// are committed or rolled back atomically.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Commits all pending changes to the database.
    /// Replaces: dbContext.SaveChanges() / dbContext.SaveChangesAsync()
    /// called directly in legacy controllers.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction for complex multi-step operations.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits the current database transaction.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back the current database transaction.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
