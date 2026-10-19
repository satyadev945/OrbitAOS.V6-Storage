// ============================================================
// Infrastructure/Repositories/Repository.cs
// Generic Repository implementation - Infrastructure layer
// Implements IRepository<T> from the Domain layer.
// Uses EF Core 8.0 (replaces EF6 / ADO.NET patterns).
// ============================================================
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;
using System.Linq.Expressions;

namespace OrbitAOS.V6.Infrastructure.Repositories;

/// <summary>
/// Generic EF Core repository implementation.
/// Provides standard CRUD operations for any entity type.
/// 
/// Migration note: In legacy ASP.NET MVC, data access was often
/// done directly in controllers using DbContext or ObjectContext.
/// This pattern centralizes data access and enables unit testing
/// via the IRepository interface.
/// </summary>
/// <typeparam name="T">Entity type managed by this repository.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    /// <inheritdoc />
    public void Remove(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }
}
