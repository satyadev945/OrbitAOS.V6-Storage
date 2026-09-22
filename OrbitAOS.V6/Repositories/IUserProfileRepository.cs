using OrbitAOS.V6.Models.Entities;

namespace OrbitAOS.V6.Repositories
{
    /// <summary>
    /// Repository interface for UserProfile entity.
    /// Follows the Repository pattern for data access abstraction.
    /// </summary>
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserProfile> AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
        Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
