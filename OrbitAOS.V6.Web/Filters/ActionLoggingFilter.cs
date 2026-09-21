using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace OrbitAOS.V6.Web.Filters;

/// <summary>
/// Global action logging filter that logs action execution details and timing.
/// Replaces legacy custom action filters from ASP.NET MVC 5 (IActionFilter).
/// Implements IAsyncActionFilter for async-first approach in .NET 8.
/// Registered globally in Program.cs via options.Filters.Add.
/// </summary>
public class ActionLoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<ActionLoggingFilter> _logger;

    public ActionLoggingFilter(ILogger<ActionLoggingFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        var stopwatch = Stopwatch.StartNew();

        _logger.LogDebug(
            "Executing action {Controller}/{Action} with parameters: {Parameters}",
            controllerName,
            actionName,
            string.Join(", ", context.ActionArguments.Select(a => $"{a.Key}={a.Value}")));

        var resultContext = await next();

        stopwatch.Stop();

        if (resultContext.Exception != null && !resultContext.ExceptionHandled)
        {
            _logger.LogWarning(
                "Action {Controller}/{Action} threw exception after {ElapsedMs}ms: {ExceptionMessage}",
                controllerName,
                actionName,
                stopwatch.ElapsedMilliseconds,
                resultContext.Exception.Message);
        }
        else
        {
            _logger.LogDebug(
                "Action {Controller}/{Action} completed in {ElapsedMs}ms.",
                controllerName,
                actionName,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
