using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Web.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Web.Controllers;

/// <summary>
/// Home controller - entry point for the application.
/// Migrated from ASP.NET MVC 5 to ASP.NET Core MVC on .NET 8.
/// Uses constructor injection for ILogger (replaces legacy static logger).
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Application home page.
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed.");
        return View();
    }

    /// <summary>
    /// Privacy policy page.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Error page - handles unhandled exceptions.
    /// ResponseCache attribute replaces legacy OutputCache.
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
