using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Application.Services
{
    /// <summary>
    /// Implementation of IUserProfileService.
    /// Contains business logic for user profile management operations.
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _repository;

        public UserProfileService(IUserProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserProfileDto?> GetProfileByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<UserProfileDto?> GetProfileByUserIdAsync(string userId)
        {
            var entity = await _repository.GetByUserIdAsync(userId);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public async Task<UserProfileDto> CreateProfileAsync(UserProfileDto dto)
        {
            var entity = MapToEntity(dto);
            entity.CreatedAt = DateTime.UtcNow;
            var created = await _repository.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task<UserProfileDto?> UpdateProfileAsync(int id, UserProfileDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.DisplayName = dto.DisplayName;
            existing.Bio = dto.Bio;
            existing.AvatarUrl = dto.AvatarUrl;
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return MapToDto(existing);
        }

        public async Task<bool> DeleteProfileAsync(int id)
        {
            if (!await _repository.ExistsAsync(id)) return false;
            await _repository.DeleteAsync(id);
            return true;
        }

        private static UserProfileDto MapToDto(UserProfile entity) => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            DisplayName = entity.DisplayName,
            Bio = entity.Bio,
            AvatarUrl = entity.AvatarUrl,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };

        private static UserProfile MapToEntity(UserProfileDto dto) => new()
        {
            UserId = dto.UserId,
            DisplayName = dto.DisplayName,
            Bio = dto.Bio,
            AvatarUrl = dto.AvatarUrl,
            IsActive = dto.IsActive
        };
    }
}
