// =============================================================================
// Infrastructure/Services/UnitOfWork.cs
// Clean Architecture: Infrastructure Layer - Unit of Work Implementation
//
// Migration Notes:
//   - Implements IUnitOfWork from the Domain layer
//   - Wraps EF Core 7 DbContext transaction management
//   - Replaces scattered SaveChanges() calls in legacy controllers
//   - Supports explicit transaction control for complex operations
// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrbitAOS.V6.Domain.Interfaces;
using OrbitAOS.V6.Infrastructure.Persistence;

namespace OrbitAOS.V6.Infrastructure.Services
{
    /// <summary>
    /// Unit of Work implementation using EF Core 7.
    /// Coordinates repository operations within a single database transaction.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <inheritdoc/>
        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to commit.");

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <inheritdoc/>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>Disposes the DbContext and any active transaction.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}
