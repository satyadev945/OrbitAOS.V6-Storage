using System.ComponentModel.DataAnnotations;

namespace OrbitAOS.V6.Domain.Entities
{
    /// <summary>
    /// Example domain entity – represents an Orbit item.
    /// Extend this with real business entities from the legacy application.
    /// </summary>
    public class OrbitItem : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public string? CreatedBy { get; set; }
    }
}
