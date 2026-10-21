// ============================================================
// Models/ErrorViewModel.cs
// Clean Architecture – Web / Presentation Layer (ViewModels)
//
// Migration note:
//   • Unchanged from net6.0 – fully compatible with .NET 8
//   • Nullable reference types (string?) retained
// ============================================================

namespace OrbitAOS.V6.Models;

/// <summary>
/// ViewModel for the Error view.
/// Carries the ASP.NET Core request trace identifier.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// The current request's trace identifier.
    /// Populated from <c>Activity.Current?.Id</c> or
    /// <c>HttpContext.TraceIdentifier</c>.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Returns <c>true</c> when a non-empty <see cref="RequestId"/> is available
    /// and should be displayed to the user.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
