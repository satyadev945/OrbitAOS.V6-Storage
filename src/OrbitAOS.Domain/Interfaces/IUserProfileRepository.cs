using OrbitAOS.Domain.Entities;

namespace OrbitAOS.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for UserProfile entity operations.
    /// Extends the generic repository with user-profile-specific queries.
    /// </summary>
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<IEnumerable<UserProfile>> GetActiveProfilesAsync();
    }
}
