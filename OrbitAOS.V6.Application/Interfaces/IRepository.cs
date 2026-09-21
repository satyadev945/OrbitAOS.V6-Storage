namespace OrbitAOS.V6.Application.Interfaces;

/// <summary>
/// Generic repository interface for data access operations.
/// Provides async CRUD operations following .NET 8 async-first patterns.
/// Replaces direct DbContext usage in legacy ASP.NET MVC controllers.
/// Implemented in the Infrastructure layer (Repository pattern).
/// </summary>
/// <typeparam name="T">Entity type (must be a reference type).</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Gets an entity by its primary key. Returns null if not found.</summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>Gets all entities (respects global query filters such as soft-delete).</summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>Adds a new entity to the context. Call SaveChanges via IUnitOfWork.</summary>
    Task<T> AddAsync(T entity);

    /// <summary>Updates an existing entity in the context. Call SaveChanges via IUnitOfWork.</summary>
    Task UpdateAsync(T entity);

    /// <summary>Removes an entity by its primary key. Call SaveChanges via IUnitOfWork.</summary>
    Task DeleteAsync(int id);

    /// <summary>Checks whether an entity with the given ID exists.</summary>
    Task<bool> ExistsAsync(int id);
}
