using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Models.Entities;

namespace OrbitAOS.V6.Repositories
{
    /// <summary>
    /// EF Core 8 implementation of the IUserProfileRepository interface.
    /// Provides data access operations for UserProfile entities.
    /// </summary>
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public UserProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
        }

        public async Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<UserProfile> AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
        {
            await _context.UserProfiles.AddAsync(userProfile, cancellationToken);
            return userProfile;
        }

        public Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
        {
            _context.UserProfiles.Update(userProfile);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var profile = await _context.UserProfiles.FindAsync(new object[] { id }, cancellationToken);
            if (profile is not null)
            {
                _context.UserProfiles.Remove(profile);
            }
        }
    }
}
