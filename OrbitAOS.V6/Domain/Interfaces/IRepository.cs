namespace OrbitAOS.V6.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface – Domain layer contract.
    /// Implementations live in the Infrastructure layer.
    /// Replaces direct DbContext usage in controllers (legacy pattern).
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>Returns all entities.</summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>Returns a single entity by primary key.</summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>Adds a new entity.</summary>
        Task AddAsync(T entity);

        /// <summary>Updates an existing entity.</summary>
        void Update(T entity);

        /// <summary>Removes an entity.</summary>
        void Remove(T entity);

        /// <summary>Persists all pending changes to the database.</summary>
        Task<int> SaveChangesAsync();
    }
}
