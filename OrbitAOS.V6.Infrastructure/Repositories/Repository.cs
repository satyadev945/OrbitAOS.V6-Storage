using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Infrastructure.Data;

namespace OrbitAOS.V6.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation using EF Core 8.
/// Provides async CRUD operations for any entity type.
/// Replaces direct DbContext usage in legacy ASP.NET MVC controllers.
/// Respects global query filters (e.g., soft-delete) defined in ApplicationDbContext.
/// </summary>
/// <typeparam name="T">Entity type (must be a reference type).</typeparam>
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
    /// <remarks>
    /// Uses FirstOrDefaultAsync with primary key predicate instead of FindAsync.
    /// FindAsync bypasses global query filters (e.g., soft-delete IsDeleted filter).
    /// FirstOrDefaultAsync respects all configured global query filters in ApplicationDbContext.
    /// Fix for issue-18: behavioral inconsistency between tests and production.
    /// </remarks>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        // Use FirstOrDefaultAsync to respect global query filters (e.g., soft-delete).
        // FindAsync bypasses global query filters by default in EF Core.
        return await _dbSet.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <inheritdoc />
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    /// <inheritdoc />
    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        return entity != null;
    }
}
