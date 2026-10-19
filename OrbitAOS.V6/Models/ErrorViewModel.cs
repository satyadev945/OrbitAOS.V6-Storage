// ============================================================
// Models/ErrorViewModel.cs
// View model for the Error view.
// Unchanged from .NET 6 - compatible with .NET 8.
// ============================================================
namespace OrbitAOS.V6.Models;

/// <summary>
/// View model for the shared Error view.
/// Carries the request ID for diagnostic display.
/// </summary>
public class ErrorViewModel
{
    /// <summary>The current request trace identifier.</summary>
    public string? RequestId { get; set; }

    /// <summary>True when RequestId has a value and should be displayed.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
