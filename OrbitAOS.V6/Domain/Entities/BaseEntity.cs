// ============================================================
// Domain/Entities/BaseEntity.cs
// Base entity class for all domain entities.
// Clean Architecture: Domain entities are plain C# objects
// with no framework dependencies (no System.Web, no EF attributes
// in the domain layer itself).
// ============================================================
namespace OrbitAOS.V6.Domain.Entities;

/// <summary>
/// Base class for all domain entities.
/// Provides common audit fields (CreatedAt, UpdatedAt).
/// Replaces legacy model classes that mixed domain logic
/// with data annotations and EF6 configuration.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key identifier.</summary>
    public int Id { get; set; }

    /// <summary>UTC timestamp when the entity was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when the entity was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Soft-delete flag. True means the record is logically deleted.</summary>
    public bool IsDeleted { get; set; } = false;
}
