using Microsoft.AspNetCore.Identity;

namespace OrbitAOS.V6.Infrastructure.Identity;

/// <summary>
/// Custom application user extending ASP.NET Core IdentityUser.
/// Adds application-specific profile properties.
/// Migrated from ASP.NET Identity 2.x ApplicationUser to ASP.NET Core Identity 8.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>User's first name.</summary>
    public string? FirstName { get; set; }

    /// <summary>User's last name.</summary>
    public string? LastName { get; set; }

    /// <summary>UTC timestamp when the user account was created.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the user's last successful login.</summary>
    public DateTime? LastLoginDate { get; set; }

    /// <summary>Indicates whether the user account is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Full display name combining first and last name.</summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
