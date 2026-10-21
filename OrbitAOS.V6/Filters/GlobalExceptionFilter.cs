// =============================================================================
// Filters/GlobalExceptionFilter.cs
// Clean Architecture: Web Layer (Presentation) - Action Filter
//
// Migration Notes:
//   - Replaces legacy HandleErrorAttribute from System.Web.Mvc
//   - Implements IAsyncExceptionFilter (.NET 7 interface)
//   - Registered globally in Program.cs via AddControllersWithViews(options => ...)
//   - Replaces App_Start/FilterConfig.cs GlobalFilters.Filters.Add()
//
// Legacy pattern (App_Start/FilterConfig.cs):
//   public static void RegisterGlobalFilters(GlobalFilterCollection filters)
//   {
//       filters.Add(new HandleErrorAttribute());
//   }
//
// .NET 7 pattern: IAsyncExceptionFilter registered in Program.cs
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace OrbitAOS.V6.Filters
{
    /// <summary>
    /// Global exception filter for MVC controllers.
    /// Migration: Replaces HandleErrorAttribute and App_Start/FilterConfig.cs.
    /// </summary>
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionFilter(
            ILogger<GlobalExceptionFilter> logger,
            IModelMetadataProvider modelMetadataProvider,
            IHostEnvironment environment)
        {
            _logger = logger;
            _modelMetadataProvider = modelMetadataProvider;
            _environment = environment;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError(context.Exception,
                "Unhandled exception in controller action. Controller: {Controller}, Action: {Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);

            if (!_environment.IsDevelopment())
            {
                // In production, show a friendly error view
                var result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new ViewDataDictionary(_modelMetadataProvider,
                        context.ModelState)
                    {
                        Model = new Models.ErrorViewModel
                        {
                            RequestId = context.HttpContext.TraceIdentifier
                        }
                    }
                };

                context.Result = result;
                context.ExceptionHandled = true;
            }

            await Task.CompletedTask;
        }
    }
}
