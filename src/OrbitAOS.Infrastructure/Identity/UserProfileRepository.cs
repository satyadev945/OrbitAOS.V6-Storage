using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Services;

namespace OrbitAOS.Infrastructure.Identity;

/// <summary>
/// EF Core repository implementation for <see cref="UserProfile"/> entities.
/// </summary>
public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    /// <summary>
    /// Initializes a new instance of <see cref="UserProfileRepository"/>.
    /// </summary>
    public UserProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByIdentityUserIdAsync(
        string identityUserId,
        CancellationToken cancellationToken = default)
        => await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, cancellationToken);
}
