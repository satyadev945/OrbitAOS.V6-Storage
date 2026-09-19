namespace OrbitAOS.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern interface for coordinating multiple repository operations.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
