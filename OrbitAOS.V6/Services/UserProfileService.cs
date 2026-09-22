using OrbitAOS.V6.Data;
using OrbitAOS.V6.Models;
using OrbitAOS.V6.Models.DTOs;
using OrbitAOS.V6.Models.Entities;
using OrbitAOS.V6.Repositories;

namespace OrbitAOS.V6.Services
{
    /// <summary>
    /// Service implementing business logic for UserProfile operations.
    /// Orchestrates data access via the repository and applies business rules.
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            IUserProfileRepository repository,
            ApplicationDbContext context,
            ILogger<UserProfileService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
        }

        public async Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving user profile with ID: {Id}", id);
            var profile = await _repository.GetByIdAsync(id, cancellationToken);
            return profile is null ? null : MapToDto(profile);
        }

        public async Task<UserProfileDto?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving user profile for user: {UserId}", userId);
            var profile = await _repository.GetByUserIdAsync(userId, cancellationToken);
            return profile is null ? null : MapToDto(profile);
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all user profiles");
            var profiles = await _repository.GetAllAsync(cancellationToken);
            return profiles.Select(MapToDto);
        }

        public async Task<UserProfileDto> CreateAsync(CreateUserProfileDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating user profile for user: {UserId}", dto.UserId);

            var existing = await _repository.GetByUserIdAsync(dto.UserId, cancellationToken);
            if (existing is not null)
            {
                throw new DomainException($"A profile already exists for user '{dto.UserId}'.");
            }

            var profile = new UserProfile
            {
                UserId = dto.UserId,
                DisplayName = dto.DisplayName,
                Bio = dto.Bio,
                AvatarUrl = dto.AvatarUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(profile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User profile created with ID: {Id}", created.Id);
            return MapToDto(created);
        }

        public async Task UpdateAsync(int id, CreateUserProfileDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating user profile with ID: {Id}", id);

            var profile = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new DomainException($"User profile with ID '{id}' was not found.");

            profile.DisplayName = dto.DisplayName;
            profile.Bio = dto.Bio;
            profile.AvatarUrl = dto.AvatarUrl;
            profile.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(profile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User profile with ID: {Id} updated successfully", id);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting user profile with ID: {Id}", id);

            var profile = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new DomainException($"User profile with ID '{id}' was not found.");

            await _repository.DeleteAsync(profile.Id, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User profile with ID: {Id} deleted successfully", id);
        }

        private static UserProfileDto MapToDto(UserProfile profile) => new()
        {
            Id = profile.Id,
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            IsActive = profile.IsActive,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt
        };
    }
}
