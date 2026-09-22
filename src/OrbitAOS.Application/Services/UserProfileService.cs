using Microsoft.Extensions.Logging;
using OrbitAOS.Application.DTOs;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;

namespace OrbitAOS.Application.Services;

/// <summary>
/// Implements business logic for user profile operations.
/// </summary>
public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _repository;
    private readonly ILogger<UserProfileService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="UserProfileService"/>.
    /// </summary>
    public UserProfileService(IUserProfileRepository repository, ILogger<UserProfileService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving user profile with ID {Id}", id);
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<UserProfileDto?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving user profile for identity user {IdentityUserId}", identityUserId);
        var entity = await _repository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);
        return entity is null ? null : MapToDto(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserProfileDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all user profiles");
        var entities = await _repository.GetAllAsync(cancellationToken);
        return entities.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public async Task<UserProfileDto> CreateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating user profile for identity user {IdentityUserId}", dto.IdentityUserId);
        var entity = MapToEntity(dto);
        var created = await _repository.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(UserProfileDto dto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating user profile with ID {Id}", dto.Id);
        var entity = MapToEntity(dto);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting user profile with ID {Id}", id);
        await _repository.DeleteAsync(id, cancellationToken);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static UserProfileDto MapToDto(UserProfile entity) => new()
    {
        Id = entity.Id,
        IdentityUserId = entity.IdentityUserId,
        DisplayName = entity.DisplayName,
        Email = entity.Email,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt
    };

    private static UserProfile MapToEntity(UserProfileDto dto) => new()
    {
        Id = dto.Id,
        IdentityUserId = dto.IdentityUserId,
        DisplayName = dto.DisplayName,
        Email = dto.Email,
        IsActive = dto.IsActive,
        CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt
    };
}
