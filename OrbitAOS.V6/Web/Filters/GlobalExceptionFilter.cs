// ============================================================
// Web/Filters/GlobalExceptionFilter.cs
// Global exception filter — replaces legacy FilterConfig.cs HandleErrorAttribute
// Clean Architecture Web Layer
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrbitAOS.V6.Models;

namespace OrbitAOS.V6.Web.Filters;

/// <summary>
/// Global exception filter registered in Program.cs via AddControllersWithViews().
/// Replaces the legacy <c>FilterConfig.RegisterGlobalFilters</c> / <c>HandleErrorAttribute</c>.
/// </summary>
public sealed class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    /// <inheritdoc />
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception,
            "Unhandled exception in {Controller}.{Action}",
            context.RouteData.Values["controller"],
            context.RouteData.Values["action"]);

        if (!_env.IsDevelopment())
        {
            // Return a friendly error view in production
            context.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                    new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                    context.ModelState)
                {
                    Model = new ErrorViewModel
                    {
                        RequestId = context.HttpContext.TraceIdentifier
                    }
                }
            };
            context.ExceptionHandled = true;
        }
    }
}
