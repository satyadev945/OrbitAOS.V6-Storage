using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;

namespace OrbitAOS.V6.Infrastructure.Repositories
{
    /// <summary>
    /// Generic EF Core 8 repository implementation – Infrastructure layer.
    /// Replaces direct DbContext calls scattered across legacy controllers.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        /// <inheritdoc />
        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        /// <inheritdoc />
        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        /// <inheritdoc />
        public void Update(T entity)
            => _dbSet.Update(entity);

        /// <inheritdoc />
        public void Remove(T entity)
            => _dbSet.Remove(entity);

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
