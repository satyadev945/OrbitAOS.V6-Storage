using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OrbitAOS.V6.Web.Filters;

/// <summary>
/// Global exception filter that catches unhandled exceptions and returns a standardized error response.
/// Replaces Application_Error event handler in legacy Global.asax.
/// Registered globally in Program.cs via options.Filters.Add.
/// In development, exceptions are passed through to the developer exception page.
/// In production, redirects to the Error action.
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionFilter(
        ILogger<GlobalExceptionFilter> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(
            context.Exception,
            "Unhandled exception occurred. RequestPath: {RequestPath}, Method: {Method}",
            context.HttpContext.Request.Path,
            context.HttpContext.Request.Method);

        if (_environment.IsDevelopment())
        {
            // In development, let the developer exception page handle it
            return;
        }

        // In production, redirect to the error page
        context.Result = new RedirectToActionResult("Error", "Home", null);
        context.ExceptionHandled = true;
    }
}
