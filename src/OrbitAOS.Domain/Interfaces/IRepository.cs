using OrbitAOS.Domain.Common;

namespace OrbitAOS.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface defining standard CRUD operations for domain entities.
    /// Follows the Repository pattern for data access abstraction.
    /// </summary>
    /// <typeparam name="T">The entity type that extends BaseEntity.</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
