using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserProfileRepository"/>.
/// </summary>
public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
{
    /// <summary>Initializes a new instance of <see cref="UserProfileRepository"/>.</summary>
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

    /// <inheritdoc />
    public async Task<UserProfile?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
        => await _context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
}
