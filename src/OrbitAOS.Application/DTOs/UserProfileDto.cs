namespace OrbitAOS.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for UserProfile.
    /// Used to transfer user profile data between application layers.
    /// </summary>
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
