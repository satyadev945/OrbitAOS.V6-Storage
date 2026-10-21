// ============================================================
// Web/Controllers/HomeController.cs
// Upgraded MVC Controller — Clean Architecture Web Layer
// Migrated from .NET 6 to .NET 8 with async actions and DI
// ============================================================

using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Web.Controllers;

/// <summary>
/// Home controller — thin controller delegating to <see cref="IHomeService"/>.
/// Follows Clean Architecture: Web layer depends on Application layer interfaces.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomeService _homeService;

    public HomeController(ILogger<HomeController> logger, IHomeService homeService)
    {
        _logger = logger;
        _homeService = homeService;
    }

    /// <summary>
    /// GET /Home/Index — Home page.
    /// Async action (replaces synchronous IActionResult in .NET 6 template).
    /// </summary>
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var message = await _homeService.GetWelcomeMessageAsync(cancellationToken);
        var viewModel = new HomeViewModel
        {
            WelcomeMessage = message,
            GeneratedAt = DateTime.UtcNow
        };
        return View(viewModel);
    }

    /// <summary>
    /// GET /Home/Privacy — Privacy policy page.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// GET /Home/Error — Error page.
    /// ResponseCache prevents caching of error responses.
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
