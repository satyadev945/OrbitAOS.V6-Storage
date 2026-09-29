namespace OrbitAOS.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a user profile for application layer operations.
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>Gets or sets the unique identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the ASP.NET Core Identity user ID.</summary>
        public string IdentityUserId { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name.</summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>Gets or sets the email address.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets whether the profile is active.</summary>
        public bool IsActive { get; set; }

        /// <summary>Gets or sets the UTC creation timestamp.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Gets or sets the UTC last-updated timestamp.</summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
