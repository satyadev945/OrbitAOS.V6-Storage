// ============================================================
// Domain/Entities/ApplicationUser.cs
// Clean Architecture – Domain Layer
//
// Migration note:
//   Legacy: IdentityUser (bare) registered in Program.cs
//   .NET 8 : Custom ApplicationUser extends IdentityUser so
//            domain-specific profile fields can be added without
//            touching the Identity schema.
// ============================================================

using Microsoft.AspNetCore.Identity;

namespace OrbitAOS.V6.Domain.Entities;

/// <summary>
/// Application-specific user entity.
/// Extends <see cref="IdentityUser"/> to allow domain profile fields.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>User's display name (optional).</summary>
    public string? DisplayName { get; set; }

    /// <summary>UTC timestamp when the account was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>UTC timestamp of the last profile update.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Soft-delete flag – user is deactivated but not removed.</summary>
    public bool IsActive { get; set; } = true;
}
