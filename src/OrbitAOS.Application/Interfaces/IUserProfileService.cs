using OrbitAOS.Application.DTOs;

namespace OrbitAOS.Application.Interfaces
{
    /// <summary>
    /// Service interface for UserProfile business operations.
    /// Defines the application-level contract for user profile management.
    /// </summary>
    public interface IUserProfileService
    {
        Task<UserProfileDto?> GetProfileByIdAsync(int id);
        Task<UserProfileDto?> GetProfileByUserIdAsync(string userId);
        Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync();
        Task<UserProfileDto> CreateProfileAsync(UserProfileDto dto);
        Task<UserProfileDto?> UpdateProfileAsync(int id, UserProfileDto dto);
        Task<bool> DeleteProfileAsync(int id);
    }
}
