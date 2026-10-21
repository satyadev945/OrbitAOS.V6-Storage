// =============================================================================
// Data/ApplicationDbContext.cs
// Web Layer - Backward Compatibility Alias
//
// Migration Notes:
//   - This file provides a namespace alias for backward compatibility
//   - The actual DbContext implementation has been moved to:
//     Infrastructure/Persistence/ApplicationDbContext.cs
//   - This alias ensures existing references in the Web layer continue to work
//   - In a full Clean Architecture migration, direct DbContext references
//     in the Web layer should be replaced with IApplicationDbContext
// =============================================================================

// Re-export the Infrastructure DbContext under the legacy Data namespace
// for backward compatibility during migration.
global using ApplicationDbContextAlias = OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext;

namespace OrbitAOS.V6.Data
{
    /// <summary>
    /// Backward-compatibility alias for ApplicationDbContext.
    /// Points to the Infrastructure layer implementation.
    /// 
    /// MIGRATION NOTE: Replace all usages of OrbitAOS.V6.Data.ApplicationDbContext
    /// with OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext
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
