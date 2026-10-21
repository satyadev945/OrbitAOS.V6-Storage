// ============================================================
// Models/ErrorViewModel.cs
// Unchanged — compatible with .NET 8
// ============================================================

namespace OrbitAOS.V6.Models;

/// <summary>
/// ViewModel for the Error view.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
