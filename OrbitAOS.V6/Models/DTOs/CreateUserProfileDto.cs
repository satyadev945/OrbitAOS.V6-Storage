using System.ComponentModel.DataAnnotations;

namespace OrbitAOS.V6.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating or updating a UserProfile.
    /// Contains validation attributes for input validation.
    /// </summary>
    public class CreateUserProfileDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Display name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Display name must be between 2 and 100 characters.")]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
        public string? Bio { get; set; }

        [Url(ErrorMessage = "Avatar URL must be a valid URL.")]
        public string? AvatarUrl { get; set; }
    }
}
