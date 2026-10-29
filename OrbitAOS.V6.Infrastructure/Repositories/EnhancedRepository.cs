using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Domain.Common;
using OrbitAOS.V6.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrbitAOS.V6.Infrastructure.Repositories
{
    /// <summary>
    /// Enhanced generic repository implementation with advanced querying capabilities
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class EnhancedRepository<T> : Repository<T>, IRepository<T> where T : BaseEntity
    {
        public EnhancedRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get entities with filtering, sorting, and pagination
        /// </summary>
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "",
            int? skip = null,
            int? take = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get count of entities matching filter
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        /// <summary>
        /// Check if any entity matches the filter
        /// </summary>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }

        /// <summary>
        /// Get first entity matching filter or null
        /// </summary>
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.FirstOrDefaultAsync(filter);
        }
    }
}
