// ============================================================
// Domain/Interfaces/IUnitOfWork.cs
// Clean Architecture – Domain Layer
//
// Unit-of-Work pattern ensures that multiple repository
// operations participate in a single database transaction.
// ============================================================

namespace OrbitAOS.V6.Domain.Interfaces;

/// <summary>
/// Coordinates multiple repository operations in one transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>Commits all pending changes to the database.</summary>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back any uncommitted changes.</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
