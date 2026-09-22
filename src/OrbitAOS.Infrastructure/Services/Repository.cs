using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Common;
using OrbitAOS.Domain.Interfaces;
using OrbitAOS.Infrastructure.Data;

namespace OrbitAOS.Infrastructure.Services;

/// <summary>
/// Generic EF Core 8 repository implementation for domain entities.
/// Replaces the legacy EF6 repository pattern with async-first EF Core 8 operations.
/// </summary>
/// <typeparam name="T">The entity type, must derive from <see cref="BaseEntity"/>.</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    /// <summary>The underlying EF Core 8 database context.</summary>
    protected readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="Repository{T}"/>.
    /// </summary>
    public Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Set<T>().FindAsync([id], cancellationToken);

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
