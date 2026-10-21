// =============================================================================
// Domain/Interfaces/IRepository.cs
// Clean Architecture: Domain Layer - Repository Interface
//
// Migration Notes:
//   - Replaces direct DbContext usage in legacy MVC controllers
//   - Provides abstraction for data access (Infrastructure layer)
//   - Enables unit testing with mock repositories
// =============================================================================

namespace OrbitAOS.V6.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for the Domain layer.
    /// Provides CRUD operations abstracted from the data store.
    /// </summary>
    /// <typeparam name="T">The entity type (must be a class)</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>Gets all entities asynchronously.</summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>Gets an entity by its primary key asynchronously.</summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>Adds a new entity asynchronously.</summary>
        Task AddAsync(T entity);

        /// <summary>Updates an existing entity.</summary>
        void Update(T entity);

        /// <summary>Removes an entity.</summary>
        void Remove(T entity);

        /// <summary>Checks if any entity satisfies the condition.</summary>
        Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        /// <summary>Finds entities matching the predicate.</summary>
        Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    }
}
