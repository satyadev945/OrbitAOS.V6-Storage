// =============================================================================
// Domain/Entities/BaseEntity.cs
// Clean Architecture: Domain Layer - Base Entity
//
// Migration Notes:
//   - Provides common audit fields for all domain entities
//   - Replaces ad-hoc timestamp fields in legacy EF6 models
//   - Supports soft-delete pattern (IsDeleted flag)
// =============================================================================

namespace OrbitAOS.V6.Domain.Entities
{
    /// <summary>
    /// Base entity class providing common audit properties for all domain entities.
    /// All domain entities should inherit from this class.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>Primary key identifier.</summary>
        public int Id { get; set; }

        /// <summary>UTC timestamp when the entity was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>UTC timestamp when the entity was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Soft-delete flag. When true, the entity is logically deleted
        /// but remains in the database for audit purposes.
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
