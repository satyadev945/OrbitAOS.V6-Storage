// ============================================================
// Controllers/HomeController.cs
// NOTE: This file is superseded by Web/Controllers/HomeController.cs
// in the Clean Architecture layout.
// Kept here for backward compatibility during migration.
// After migration is complete, remove this file and update routing
// to use the Web/Controllers namespace.
// ============================================================

// See Web/Controllers/HomeController.cs for the upgraded implementation.
// This stub prevents build errors during the transition period.

using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers;

/// <summary>
/// Legacy controller location — delegates to Application layer services.
/// Migrate to <see cref="OrbitAOS.V6.Web.Controllers.HomeController"/> when ready.
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

    /// <summary>GET /Home/Index — async action (upgraded from sync in .NET 6 template).</summary>
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

    /// <summary>GET /Home/Privacy.</summary>
    public IActionResult Privacy() => View();

    /// <summary>GET /Home/Error — no-cache error page.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
