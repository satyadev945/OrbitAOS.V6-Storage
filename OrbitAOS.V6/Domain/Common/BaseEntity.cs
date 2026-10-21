// ============================================================
// Domain/Common/BaseEntity.cs
// Base entity for all domain entities — Clean Architecture Domain Layer
// ============================================================

namespace OrbitAOS.V6.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides common audit fields and identity.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>UTC timestamp when the entity was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when the entity was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Soft-delete flag — entities are never physically removed.</summary>
    public bool IsDeleted { get; set; } = false;
}
