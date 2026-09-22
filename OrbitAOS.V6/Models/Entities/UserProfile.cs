using OrbitAOS.V6.Models.Entities;

namespace OrbitAOS.V6.Models.Entities
{
    /// <summary>
    /// Represents an application user profile entity.
    /// Stores extended profile information linked to an ASP.NET Core Identity user.
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
