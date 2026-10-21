// ============================================================
// Data/ApplicationDbContext.cs
// MIGRATION NOTE: This file is superseded by
//   Infrastructure/Data/ApplicationDbContext.cs
// in the Clean Architecture layout.
//
// This stub re-exports the Infrastructure DbContext so that
// existing EF Core migration files (Data/Migrations/) continue
// to compile without modification during the transition.
// ============================================================

// Re-export the Infrastructure DbContext under the legacy namespace
// so existing migration files referencing OrbitAOS.V6.Data.ApplicationDbContext
// continue to compile.

namespace OrbitAOS.V6.Data;

/// <summary>
/// Compatibility alias — delegates to <see cref="OrbitAOS.V6.Infrastructure.Data.ApplicationDbContext"/>.
/// Remove this file once all migration files are regenerated targeting the Infrastructure namespace.
/// </summary>
public class ApplicationDbContext : OrbitAOS.V6.Infrastructure.Data.ApplicationDbContext
{
    public ApplicationDbContext(
        Microsoft.EntityFrameworkCore.DbContextOptions<OrbitAOS.V6.Infrastructure.Data.ApplicationDbContext> options)
        : base(options)
    {
    }
}
