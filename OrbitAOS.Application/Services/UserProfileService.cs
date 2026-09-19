using Microsoft.Extensions.Logging;
using OrbitAOS.Application.Common.Models;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Application.Services
{
    /// <summary>
    /// Implementation of user profile service using repository pattern.
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            IRepository<UserProfile> userProfileRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserProfileService> logger)
        {
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<UserProfileDto>> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
        {
            try
            {
                var profiles = await _userProfileRepository.FindAsync(
                    p => p.IdentityUserId == identityUserId, cancellationToken);
                var profile = profiles.FirstOrDefault();

                if (profile == null)
                    return Result<UserProfileDto>.Failure($"User profile not found for identity user: {identityUserId}");

                return Result<UserProfileDto>.Success(MapToDto(profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile for identity user {IdentityUserId}", identityUserId);
                return Result<UserProfileDto>.Failure("An error occurred while retrieving the user profile.");
            }
        }

        public async Task<Result<UserProfileDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var profile = await _userProfileRepository.GetByIdAsync(id, cancellationToken);
                if (profile == null)
                    return Result<UserProfileDto>.Failure($"User profile with ID {id} not found.");

                return Result<UserProfileDto>.Success(MapToDto(profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile {Id}", id);
                return Result<UserProfileDto>.Failure("An error occurred while retrieving the user profile.");
            }
        }

        public async Task<Result<IEnumerable<UserProfileDto>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var profiles = await _userProfileRepository.GetAllAsync(cancellationToken);
                return Result<IEnumerable<UserProfileDto>>.Success(profiles.Select(MapToDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all user profiles");
                return Result<IEnumerable<UserProfileDto>>.Failure("An error occurred while retrieving user profiles.");
            }
        }

        public async Task<Result<UserProfileDto>> CreateAsync(CreateUserProfileDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var profile = new UserProfile
                {
                    IdentityUserId = dto.IdentityUserId,
                    DisplayName = dto.DisplayName,
                    Department = dto.Department,
                    JobTitle = dto.JobTitle,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userProfileRepository.AddAsync(profile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created user profile for identity user {IdentityUserId}", dto.IdentityUserId);
                return Result<UserProfileDto>.Success(MapToDto(profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user profile for identity user {IdentityUserId}", dto.IdentityUserId);
                return Result<UserProfileDto>.Failure("An error occurred while creating the user profile.");
            }
        }

        public async Task<Result> UpdateAsync(int id, UpdateUserProfileDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var profile = await _userProfileRepository.GetByIdAsync(id, cancellationToken);
                if (profile == null)
                    return Result.Failure($"User profile with ID {id} not found.");

                profile.DisplayName = dto.DisplayName;
                profile.Department = dto.Department;
                profile.JobTitle = dto.JobTitle;
                profile.IsActive = dto.IsActive;
                profile.UpdatedAt = DateTime.UtcNow;

                await _userProfileRepository.UpdateAsync(profile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Updated user profile {Id}", id);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile {Id}", id);
                return Result.Failure("An error occurred while updating the user profile.");
            }
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var profile = await _userProfileRepository.GetByIdAsync(id, cancellationToken);
                if (profile == null)
                    return Result.Failure($"User profile with ID {id} not found.");

                await _userProfileRepository.DeleteAsync(profile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deleted user profile {Id}", id);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user profile {Id}", id);
                return Result.Failure("An error occurred while deleting the user profile.");
            }
        }

        private static UserProfileDto MapToDto(UserProfile profile) =>
            new UserProfileDto(
                profile.Id,
                profile.IdentityUserId,
                profile.DisplayName,
                profile.Department,
                profile.JobTitle,
                profile.IsActive,
                profile.CreatedAt
            );
    }
}
