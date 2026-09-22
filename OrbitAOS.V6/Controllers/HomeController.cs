using Microsoft.AspNetCore.Mvc;
using OrbitAOS.Application.Interfaces;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers;

/// <summary>
/// Handles requests for the main application pages: Home, Privacy, and Error.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserProfileService _userProfileService;

    /// <summary>Initializes a new instance of <see cref="HomeController"/>.</summary>
    public HomeController(ILogger<HomeController> logger, IUserProfileService userProfileService)
    {
        _logger = logger;
        _userProfileService = userProfileService;
    }

    /// <summary>Renders the application home page.</summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed at {Time}", DateTime.UtcNow);
        return View();
    }

    /// <summary>Renders the privacy policy page.</summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>Renders the error page. Response caching is disabled for this action.</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
