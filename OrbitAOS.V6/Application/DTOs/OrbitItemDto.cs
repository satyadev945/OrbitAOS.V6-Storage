using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for OrbitItem.
    /// Application layer – decouples domain entities from view models.
    /// </summary>
    public class OrbitItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Maps a domain entity to a DTO.</summary>
        public static OrbitItemDto FromEntity(OrbitItem entity) => new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt
        };
    }
}
