namespace OrbitAOS.V6.Application.Interfaces;

/// <summary>
/// Unit of Work pattern interface for managing database transactions.
/// Coordinates multiple repository operations within a single transaction.
/// Replaces direct SaveChanges calls scattered across legacy controllers.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Persists all pending changes to the database.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Begins a new database transaction.</summary>
    Task BeginTransactionAsync();

    /// <summary>Commits the current transaction and persists all changes.</summary>
    Task CommitTransactionAsync();

    /// <summary>Rolls back the current transaction, discarding all pending changes.</summary>
    Task RollbackTransactionAsync();
}
