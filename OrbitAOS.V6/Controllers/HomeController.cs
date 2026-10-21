// ============================================================
// Controllers/HomeController.cs
// Clean Architecture – Web / Presentation Layer
//
// Migration notes:
//   • Upgraded from net6.0 → net8.0
//   • Controller now depends on IHomeService (Application layer)
//     instead of containing business logic directly.
//   • Index action is now async (best practice for .NET 8)
//   • ILogger<T> injection retained from legacy app
// ============================================================

using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers;

/// <summary>
/// Handles requests for the Home area of the application.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomeService _homeService;

    /// <summary>
    /// Initialises a new instance of <see cref="HomeController"/>.
    /// </summary>
    /// <param name="logger">Logger injected by the DI container.</param>
    /// <param name="homeService">Home feature service (Application layer).</param>
    public HomeController(ILogger<HomeController> logger, IHomeService homeService)
    {
        _logger = logger;
        _homeService = homeService;
    }

    /// <summary>
    /// GET /  or  GET /Home/Index
    /// Renders the application home page.
    /// </summary>
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var message = await _homeService.GetWelcomeMessageAsync(cancellationToken);
        ViewData["WelcomeMessage"] = message;
        return View();
    }

    /// <summary>
    /// GET /Home/Privacy
    /// Renders the privacy policy page.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// GET /Home/Error
    /// Renders the error page.  ResponseCache prevents caching of error responses.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
