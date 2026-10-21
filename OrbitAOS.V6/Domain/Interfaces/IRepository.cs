// ============================================================
// Domain/Interfaces/IRepository.cs
// Clean Architecture – Domain Layer
//
// Generic repository contract.  Infrastructure layer provides
// the EF Core implementation; Application layer depends only
// on this interface (Dependency Inversion Principle).
// ============================================================

namespace OrbitAOS.V6.Domain.Interfaces;

/// <summary>
/// Generic repository interface for CRUD operations.
/// </summary>
/// <typeparam name="T">Domain entity type.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Returns all entities (no tracking).</summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a single entity by its primary key.</summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity to the store.</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entity.</summary>
    void Update(T entity);

    /// <summary>Removes an entity from the store.</summary>
    void Delete(T entity);

    /// <summary>Persists all pending changes to the database.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
