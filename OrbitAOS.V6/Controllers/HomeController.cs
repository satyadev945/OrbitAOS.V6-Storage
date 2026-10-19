// ============================================================
// Controllers/HomeController.cs
// Web layer - MVC Controller (.NET 8 ASP.NET Core MVC)
// Clean Architecture: Web layer depends on Application layer
// interfaces, not on Infrastructure or Data directly.
// ============================================================
using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers;

/// <summary>
/// Home controller - handles the root application routes.
/// 
/// Migration notes from legacy ASP.NET MVC:
/// - Inherits from Controller (Microsoft.AspNetCore.Mvc) not System.Web.Mvc.Controller
/// - Constructor injection replaces DependencyResolver.Current.GetService()
/// - IActionResult replaces ActionResult (IActionResult is the base interface)
/// - async/await pattern used throughout (no synchronous I/O)
/// - ILogger<T> replaces log4net / NLog direct instantiation
/// - Business logic delegated to IHomeService (Clean Architecture)
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHomeService _homeService;

    /// <summary>
    /// Constructor injection (replaces legacy DependencyResolver pattern).
    /// Dependencies are registered in Program.cs via builder.Services.
    /// </summary>
    public HomeController(ILogger<HomeController> logger, IHomeService homeService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _homeService = homeService ?? throw new ArgumentNullException(nameof(homeService));
    }

    /// <summary>
    /// GET /Home/Index (default route: /)
    /// Async action method - replaces synchronous ActionResult in legacy MVC.
    /// </summary>
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        _logger.LogInformation("HomeController.Index called");

        // Business logic delegated to Application service layer
        var welcomeMessage = await _homeService.GetWelcomeMessageAsync(cancellationToken);
        ViewData["WelcomeMessage"] = welcomeMessage;

        return View();
    }

    /// <summary>
    /// GET /Home/Privacy
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// GET /Home/Error
    /// Error handler - replaces customErrors in web.config.
    /// [ResponseCache] replaces [OutputCache] from System.Web.Mvc.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error page displayed. RequestId: {RequestId}", requestId);

        return View(new ErrorViewModel { RequestId = requestId });
    }
}
