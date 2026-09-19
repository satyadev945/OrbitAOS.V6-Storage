using OrbitAOS.Domain.Common;

namespace OrbitAOS.Domain.Entities
{
    /// <summary>
    /// Represents an application user profile linked to ASP.NET Core Identity.
    /// </summary>
    public class UserProfile : BaseEntity
    {
        public string IdentityUserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
