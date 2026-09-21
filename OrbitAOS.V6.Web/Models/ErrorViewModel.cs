namespace OrbitAOS.V6.Web.Models;

/// <summary>
/// ViewModel for the Error page.
/// Displays request ID for support and debugging purposes.
/// </summary>
public class ErrorViewModel
{
    /// <summary>The request trace identifier for support purposes.</summary>
    public string? RequestId { get; set; }

    /// <summary>Returns true if a RequestId is available to display.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
