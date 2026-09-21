using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrbitAOS.Web.Filters
{
    /// <summary>
    /// Global exception filter for handling unhandled exceptions in MVC actions.
    /// Replaces legacy HandleError attribute and custom error modules.
    /// In .NET 8, this works alongside UseExceptionHandler middleware.
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionFilter(
            ILogger<GlobalExceptionFilter> logger,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception,
                "Unhandled exception in {Controller}.{Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);

            if (!_env.IsDevelopment())
            {
                context.Result = new RedirectToActionResult("Error", "Home", null);
                context.ExceptionHandled = true;
            }
        }
    }
}
