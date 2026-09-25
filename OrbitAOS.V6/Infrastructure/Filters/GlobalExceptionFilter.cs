using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrbitAOS.V6.Infrastructure.Filters
{
    /// <summary>
    /// Global exception filter – Infrastructure layer.
    /// Replaces legacy HandleErrorAttribute and FilterConfig.cs registration.
    /// Catches unhandled exceptions and returns a structured error response.
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Unhandled exception occurred: {Message}", context.Exception.Message);

            if (_env.IsDevelopment())
            {
                // In development, let the developer exception page handle it
                return;
            }

            context.Result = new RedirectToActionResult("Error", "Home", null);
            context.ExceptionHandled = true;
        }
    }
}
