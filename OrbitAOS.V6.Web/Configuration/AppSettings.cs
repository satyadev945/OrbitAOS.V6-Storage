namespace OrbitAOS.V6.Web.Configuration;

/// <summary>
/// Strongly-typed application settings class.
/// Bound from the "AppSettings" section in appsettings.json.
/// Replaces legacy web.config appSettings key-value pairs.
/// Injected via IOptions&lt;AppSettings&gt; pattern in .NET 8.
/// </summary>
public class AppSettings
{
    /// <summary>Display name of the application.</summary>
    public string ApplicationName { get; set; } = "OrbitAOS";

    /// <summary>Current application version string.</summary>
    public string Version { get; set; } = "8.0.0";

    /// <summary>Support contact email address.</summary>
    public string SupportEmail { get; set; } = string.Empty;

    /// <summary>Current deployment environment name (Development, Staging, Production).</summary>
    public string Environment { get; set; } = "Production";
}
