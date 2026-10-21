// ============================================================
// Infrastructure/Identity/ApplicationUser.cs
// Custom Identity user — Clean Architecture Infrastructure Layer
// Replaces: direct use of IdentityUser in .NET 6 template
// ============================================================

using Microsoft.AspNetCore.Identity;

namespace OrbitAOS.V6.Infrastructure.Identity;

/// <summary>
/// Application-specific Identity user.
/// Extend this class to add custom profile properties.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>User's display name (optional).</summary>
    public string? DisplayName { get; set; }

    /// <summary>UTC timestamp of the last successful login.</summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>Whether the user account is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>UTC timestamp when the account was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
