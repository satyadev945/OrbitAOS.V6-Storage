// ============================================================
// Domain/Entities/BaseEntity.cs
// Clean Architecture – Domain Layer
//
// All domain entities inherit from BaseEntity to get a
// consistent primary key and audit timestamps.
// ============================================================

namespace OrbitAOS.V6.Domain.Entities;

/// <summary>
/// Base class for all domain entities.
/// Provides a strongly-typed primary key and audit fields.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key – auto-generated GUID.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>UTC timestamp when the record was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the last update (null if never updated).</summary>
    public DateTime? UpdatedAt { get; set; }
}
