// ============================================================
// Domain/Interfaces/IRepository.cs
// Generic repository interface — Clean Architecture Domain Layer
// ============================================================

using OrbitAOS.V6.Domain.Common;
using System.Linq.Expressions;

namespace OrbitAOS.V6.Domain.Interfaces;

/// <summary>
/// Generic repository contract for data access.
/// Implementations live in the Infrastructure layer.
/// </summary>
/// <typeparam name="T">Domain entity type derived from <see cref="BaseEntity"/>.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>Returns all non-deleted entities.</summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns entities matching the given predicate.</summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Returns a single entity by primary key, or null.</summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity to the store.</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entity.</summary>
    void Update(T entity);

    /// <summary>Soft-deletes an entity (sets IsDeleted = true).</summary>
    void Delete(T entity);

    /// <summary>Persists all pending changes to the database.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
