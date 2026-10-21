// ============================================================
// Data/ApplicationDbContext.cs
// MIGRATION NOTE: This file is a compatibility alias so that
//   existing EF Core migration files (Data/Migrations/) continue
//   to compile without modification during the transition.
//
// The canonical DbContext is Infrastructure/Data/ApplicationDbContext.cs
// ============================================================

using Microsoft.EntityFrameworkCore;

namespace OrbitAOS.V6.Data;

/// <summary>
/// Compatibility alias — delegates to <see cref="OrbitAOS.V6.Infrastructure.Data.ApplicationDbContext"/>.
/// Remove this file once all migration files are regenerated targeting the Infrastructure namespace.
/// </summary>
public class ApplicationDbContext : OrbitAOS.V6.Infrastructure.Data.ApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<OrbitAOS.V6.Infrastructure.Data.ApplicationDbContext> options)
        : base(options)
    {
    }
}
