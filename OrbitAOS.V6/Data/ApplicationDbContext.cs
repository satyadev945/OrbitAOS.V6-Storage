// =============================================================================
// Data/ApplicationDbContext.cs
// Web Layer - EF Core 8 DbContext (backward-compatibility shim)
//
// Migration Notes:
//   - Provides a Data-namespace DbContext for backward compatibility
//   - The actual implementation is in Infrastructure/Persistence/ApplicationDbContext.cs
//   - Program.cs registers OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext
// =============================================================================

namespace OrbitAOS.V6.Data
{
    /// <summary>
    /// Backward-compatibility alias for ApplicationDbContext.
    /// Points to the Infrastructure layer implementation.
    /// Replace all usages with OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext
    /// or inject IApplicationDbContext via dependency injection.
    /// </summary>
    public class ApplicationDbContext : OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext
    {
        public ApplicationDbContext(
            Microsoft.EntityFrameworkCore.DbContextOptions<OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
