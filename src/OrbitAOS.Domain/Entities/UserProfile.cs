using OrbitAOS.Domain.Common;

namespace OrbitAOS.Domain.Entities
{
    /// <summary>
    /// Represents an application user profile entity in the domain layer.
    /// Extends the base entity with user-specific properties.
    /// </summary>
    public class UserProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
