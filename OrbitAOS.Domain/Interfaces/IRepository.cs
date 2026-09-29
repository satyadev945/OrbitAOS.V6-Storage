using OrbitAOS.Domain.Common;

namespace OrbitAOS.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface defining standard CRUD operations for domain entities.
    /// </summary>
    /// <typeparam name="T">The entity type, must derive from <see cref="BaseEntity"/>.</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>Retrieves an entity by its unique identifier asynchronously.</summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>Retrieves all entities asynchronously.</summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>Adds a new entity asynchronously.</summary>
        Task<T> AddAsync(T entity);

        /// <summary>Updates an existing entity asynchronously.</summary>
        Task UpdateAsync(T entity);

        /// <summary>Deletes an entity by its unique identifier asynchronously.</summary>
        Task DeleteAsync(int id);

        /// <summary>Checks whether an entity with the given identifier exists asynchronously.</summary>
        Task<bool> ExistsAsync(int id);
    }
}
