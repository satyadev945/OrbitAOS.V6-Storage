// ============================================================
// Application/DTOs/HomeViewModel.cs
// Data Transfer Object — Clean Architecture Application Layer
// ============================================================

namespace OrbitAOS.V6.Application.DTOs;

/// <summary>
/// ViewModel / DTO for the Home page.
/// Passed from the Application layer to the Web (presentation) layer.
/// </summary>
public sealed class HomeViewModel
{
    /// <summary>Welcome message to display on the home page.</summary>
    public string WelcomeMessage { get; init; } = string.Empty;

    /// <summary>Current UTC timestamp for display purposes.</summary>
    public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
}
