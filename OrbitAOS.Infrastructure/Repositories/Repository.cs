using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Common;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Repositories
{
    /// <summary>
    /// Generic EF Core repository implementation providing standard CRUD operations.
    /// </summary>
    /// <typeparam name="T">The entity type, must derive from <see cref="BaseEntity"/>.</typeparam>
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        /// <summary>The underlying EF Core database context.</summary>
        protected readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of <see cref="Repository{T}"/>.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

        /// <inheritdoc />
        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity is not null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync(int id)
            => await _context.Set<T>().AnyAsync(e => e.Id == id);
    }
}
