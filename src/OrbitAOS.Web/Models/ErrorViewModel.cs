namespace OrbitAOS.Web.Models;

/// <summary>
/// View model for the error page, providing request tracking information.
/// Replaces the legacy ASP.NET MVC HandleErrorAttribute pattern with ASP.NET Core exception handling.
/// </summary>
public class ErrorViewModel
{
    /// <summary>Gets or sets the current request ID for diagnostics.</summary>
    public string? RequestId { get; set; }

    /// <summary>Gets a value indicating whether the request ID should be displayed.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
