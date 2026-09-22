using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers
{
    /// <summary>
    /// Home controller – Web layer.
    /// Migrated from legacy ASP.NET MVC HomeController.
    /// Uses constructor injection (replaces legacy DependencyResolver).
    /// All actions return IActionResult (replaces ActionResult).
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService _homeService;

        // Constructor injection – replaces legacy DependencyResolver / ServiceLocator
        public HomeController(ILogger<HomeController> logger, IHomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        /// <summary>
        /// GET /Home/Index
        /// Async action (replaces synchronous legacy action).
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var message = await _homeService.GetWelcomeMessageAsync();
            ViewData["WelcomeMessage"] = message;
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
        /// ResponseCache replaces legacy [OutputCache] attribute.
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
}
