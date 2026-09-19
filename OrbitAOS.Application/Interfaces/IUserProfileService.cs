using OrbitAOS.Application.Common.Models;

namespace OrbitAOS.Application.Interfaces
{
    /// <summary>
    /// Service interface for user profile management operations.
    /// </summary>
    public interface IUserProfileService
    {
        Task<Result<UserProfileDto>> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);
        Task<Result<UserProfileDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<UserProfileDto>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<UserProfileDto>> CreateAsync(CreateUserProfileDto dto, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int id, UpdateUserProfileDto dto, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Data transfer object for user profile read operations.
    /// </summary>
    public record UserProfileDto(
        int Id,
        string IdentityUserId,
        string DisplayName,
        string? Department,
        string? JobTitle,
        bool IsActive,
        DateTime CreatedAt
    );

    /// <summary>
    /// Data transfer object for creating a new user profile.
    /// </summary>
    public record CreateUserProfileDto(
        string IdentityUserId,
        string DisplayName,
        string? Department,
        string? JobTitle
    );

    /// <summary>
    /// Data transfer object for updating an existing user profile.
    /// </summary>
    public record UpdateUserProfileDto(
        string DisplayName,
        string? Department,
        string? JobTitle,
        bool IsActive
    );
}
