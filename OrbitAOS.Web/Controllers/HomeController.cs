using Microsoft.AspNetCore.Mvc;
using OrbitAOS.Web.Models;
using System.Diagnostics;

namespace OrbitAOS.Web.Controllers
{
    /// <summary>
    /// Handles the main application pages: Home, Privacy, and Error.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="HomeController"/>.
        /// </summary>
        /// <param name="logger">The logger instance injected via DI.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>Renders the application home page.</summary>
        public IActionResult Index()
        {
            _logger.LogInformation("Home page accessed.");
            return View();
        }

        /// <summary>Renders the privacy policy page.</summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>Renders the error page. Response caching is disabled for error pages.</summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
