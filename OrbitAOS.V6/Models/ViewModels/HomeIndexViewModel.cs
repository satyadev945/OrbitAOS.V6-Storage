// ============================================================
// Models/ViewModels/HomeIndexViewModel.cs
// View model for the Home/Index view.
// Clean Architecture: ViewModels live in the Web layer (Models/).
// They are populated by controllers from Application layer DTOs.
// ============================================================
namespace OrbitAOS.V6.Models.ViewModels;

/// <summary>
/// View model for the Home Index page.
/// 
/// Migration note: In legacy ASP.NET MVC, ViewBag (dynamic) was
/// commonly used to pass data to views. The recommended .NET 8
/// pattern is strongly-typed view models for compile-time safety.
/// 
/// Legacy pattern (avoid):
///   ViewBag.WelcomeMessage = "Hello";
/// 
/// .NET 8 pattern (use this):
///   return View(new HomeIndexViewModel { WelcomeMessage = "Hello" });
/// </summary>
public class HomeIndexViewModel
{
    /// <summary>Welcome message displayed on the home page.</summary>
    public string WelcomeMessage { get; set; } = string.Empty;

    /// <summary>Application version for display purposes.</summary>
    public string AppVersion { get; set; } = "8.0";

    /// <summary>Current UTC timestamp for the page render.</summary>
    public DateTime RenderedAt { get; set; } = DateTime.UtcNow;
}
