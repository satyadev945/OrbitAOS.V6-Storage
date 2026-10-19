// ============================================================
// Domain/Interfaces/IRepository.cs
// Generic repository interface - Domain layer contract
// Clean Architecture: Domain layer defines interfaces,
// Infrastructure layer provides implementations.
// ============================================================
namespace OrbitAOS.V6.Domain.Interfaces;

/// <summary>
/// Generic repository interface for data access abstraction.
/// Replaces direct DbContext usage in controllers (legacy pattern).
/// </summary>
/// <typeparam name="T">Entity type (must be a class)</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Gets an entity by its primary key.</summary>
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>Gets all entities of type T.</summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a new entity to the repository.</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entity.</summary>
    void Update(T entity);

    /// <summary>Removes an entity from the repository.</summary>
    void Remove(T entity);

    /// <summary>Checks whether any entity matching the predicate exists.</summary>
    Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
}
