// ============================================================
// Infrastructure/Persistence/GenericRepository.cs
// Clean Architecture – Infrastructure Layer
//
// EF Core implementation of the generic IRepository<T> contract
// defined in the Domain layer.
// ============================================================

using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Interfaces;

namespace OrbitAOS.V6.Infrastructure.Persistence;

/// <summary>
/// Generic EF Core repository providing standard CRUD operations.
/// </summary>
/// <typeparam name="T">Domain entity type.</typeparam>
public sealed class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initialises a new instance of <see cref="GenericRepository{T}"/>.
    /// </summary>
    /// <param name="context">The EF Core database context.</param>
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    /// <inheritdoc />
    public void Update(T entity)
        => _dbSet.Update(entity);

    /// <inheritdoc />
    public void Delete(T entity)
        => _dbSet.Remove(entity);

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
