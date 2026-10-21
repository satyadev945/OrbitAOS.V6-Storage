// =============================================================================
// Controllers/HomeController.cs
// Clean Architecture: Web Layer (Presentation) - MVC Controller
//
// Migration Notes:
//   - Upgraded from ASP.NET MVC 6.0 (net6.0) to ASP.NET Core MVC .NET 7
//   - Uses ILogger<T> via constructor injection (replaces static log4net/NLog)
//   - IActionResult return types (compatible with .NET 7)
//   - ResponseCache attribute replaces legacy [OutputCache]
//   - Activity.Current?.Id for distributed tracing (.NET 7)
//   - Clean Architecture: Controller delegates to Application layer (MediatR)
// =============================================================================

using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrbitAOS.V6.Models;
using System.Diagnostics;

namespace OrbitAOS.V6.Controllers
{
    /// <summary>
    /// Home controller for the OrbitAOS application.
    /// Follows Clean Architecture: thin controller that delegates to Application layer.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of HomeController.
        /// Dependencies are injected via ASP.NET Core DI container.
        /// Migration: Replaces legacy DependencyResolver.Current.GetService() pattern.
        /// </summary>
        /// <param name="logger">Logger instance (replaces log4net/NLog static loggers).</param>
        /// <param name="mediator">MediatR mediator for CQRS command/query dispatch.</param>
        public HomeController(ILogger<HomeController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// GET: /Home/Index
        /// Application home page.
        /// Migration: Synchronous action → async/await pattern (.NET 7 best practice).
        /// </summary>
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Home/Index accessed at {Time}", DateTime.UtcNow);
            // Example: var result = await _mediator.Send(new GetDashboardQuery());
            return View();
        }

        /// <summary>
        /// GET: /Home/Privacy
        /// Privacy policy page.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// GET: /Home/Error
        /// Error page with request tracking.
        /// Migration: [OutputCache] → [ResponseCache] with NoStore=true for error pages.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Activity.Current?.Id provides distributed tracing correlation ID (.NET 7)
            // Migration: Replaces legacy Request.ServerVariables["HTTP_X_CORRELATION_ID"]
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
