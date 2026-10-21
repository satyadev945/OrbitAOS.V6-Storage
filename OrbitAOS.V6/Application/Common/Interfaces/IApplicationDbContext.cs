// =============================================================================
// Application/Common/Interfaces/IApplicationDbContext.cs
// Clean Architecture: Application Layer - DbContext Interface
//
// Migration Notes:
//   - Abstracts the DbContext from the Application layer
//   - Enables unit testing without a real database
//   - Follows Clean Architecture dependency inversion principle
// =============================================================================

using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for the application database context.
    /// The Application layer depends on this abstraction, not the concrete EF Core DbContext.
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>Saves all changes made in this context to the database.</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
