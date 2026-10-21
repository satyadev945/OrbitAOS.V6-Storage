// =============================================================================
// Infrastructure/Repositories/Repository.cs
// Clean Architecture: Infrastructure Layer - Generic Repository Implementation
//
// Migration Notes:
//   - Implements IRepository<T> from the Domain layer
//   - Replaces direct DbContext usage in legacy MVC controllers
//   - Uses EF Core 8 async methods (FindAsync, ToListAsync, etc.)
//   - Supports expression-based filtering
// =============================================================================

using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace OrbitAOS.V6.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation using EF Core 8.
    /// Provides standard CRUD operations for any entity type.
    /// </summary>
    /// <typeparam name="T">The entity type (must be a class).</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <inheritdoc/>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <inheritdoc/>
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
