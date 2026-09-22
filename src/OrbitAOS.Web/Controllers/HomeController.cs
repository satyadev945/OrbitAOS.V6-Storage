using Microsoft.AspNetCore.Mvc;
using OrbitAOS.Web.Models;
using System.Diagnostics;

namespace OrbitAOS.Web.Controllers;

/// <summary>
/// Home controller providing the main landing page, privacy page, and error handling.
/// Migrated from legacy ASP.NET MVC to ASP.NET Core MVC on .NET 8 with Clean Architecture wiring.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="HomeController"/> with injected logger.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic output.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>Returns the application home page.</summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed");
        return View();
    }

    /// <summary>Returns the privacy policy page.</summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>Returns the error page with request tracking information.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
