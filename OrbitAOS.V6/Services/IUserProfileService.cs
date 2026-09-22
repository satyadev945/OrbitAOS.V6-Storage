using OrbitAOS.V6.Models.DTOs;

namespace OrbitAOS.V6.Services
{
    /// <summary>
    /// Service interface for UserProfile operations.
    /// Defines the contract for business logic related to user profiles.
    /// </summary>
    public interface IUserProfileService
    {
        Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserProfileDto?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserProfileDto> CreateAsync(CreateUserProfileDto dto, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, CreateUserProfileDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
