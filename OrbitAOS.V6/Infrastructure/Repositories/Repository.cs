// ============================================================
// Infrastructure/Repositories/Repository.cs
// Generic EF Core repository — Clean Architecture Infrastructure Layer
// ============================================================

using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Common;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrbitAOS.V6.Infrastructure.Repositories;

/// <summary>
/// Generic EF Core repository implementation.
/// Registered in DI as <c>IRepository&lt;T&gt;</c>.
/// </summary>
/// <typeparam name="T">Domain entity type.</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    /// <inheritdoc />
    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }

    /// <inheritdoc />
    public void Delete(T entity)
    {
        // Soft delete — never physically remove records
        entity.IsDeleted = true;
        Update(entity);
    }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
