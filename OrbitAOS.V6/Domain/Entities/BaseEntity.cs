namespace OrbitAOS.V6.Domain.Entities
{
    /// <summary>
    /// Base entity class for all domain entities.
    /// Domain layer – contains pure business objects with no framework dependencies.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>Primary key identifier.</summary>
        public int Id { get; set; }

        /// <summary>UTC timestamp when the entity was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>UTC timestamp when the entity was last updated.</summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
