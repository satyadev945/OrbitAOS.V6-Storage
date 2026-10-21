// =============================================================================
// Domain/Interfaces/IUnitOfWork.cs
// Clean Architecture: Domain Layer - Unit of Work Interface
//
// Migration Notes:
//   - Coordinates multiple repository operations in a single transaction
//   - Replaces direct SaveChanges() calls scattered across legacy controllers
//   - Enables atomic operations across multiple repositories
// =============================================================================

namespace OrbitAOS.V6.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work interface for coordinating repository transactions.
    /// Ensures all changes are committed or rolled back atomically.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commits all pending changes to the database asynchronously.
        /// </summary>
        /// <returns>Number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current database transaction.
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current database transaction.
        /// </summary>
        Task RollbackTransactionAsync();
    }
}
