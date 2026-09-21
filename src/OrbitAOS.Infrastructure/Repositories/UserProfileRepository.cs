using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for UserProfile entity.
    /// Extends the generic repository with user-profile-specific queries.
    /// </summary>
    public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<UserProfile>> GetActiveProfilesAsync()
        {
            return await _dbSet.Where(p => p.IsActive).ToListAsync();
        }
    }
}
