namespace OrbitAOS.V6.Domain.Common;

/// <summary>
/// Base entity class with common audit properties.
/// All domain entities inherit from this class.
/// Provides Id, audit timestamps, soft-delete support.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Primary key identifier.</summary>
    public int Id { get; set; }

    /// <summary>UTC timestamp when the entity was created.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp when the entity was last modified.</summary>
    public DateTime? ModifiedDate { get; set; }

    /// <summary>Username or identifier of the user who created the entity.</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Username or identifier of the user who last modified the entity.</summary>
    public string? ModifiedBy { get; set; }

    /// <summary>Soft-delete flag. When true, the entity is excluded from default queries.</summary>
    public bool IsDeleted { get; set; } = false;
}
