// ============================================================
// Web/Middleware/RequestLoggingMiddleware.cs
// Custom middleware — replaces legacy IHttpModule implementations
// Clean Architecture Web Layer
// ============================================================

namespace OrbitAOS.V6.Web.Middleware;

/// <summary>
/// Custom request logging middleware.
/// Replaces legacy HTTP Modules (IHttpModule) from ASP.NET MVC on .NET Framework.
/// Registered in Program.cs via <c>app.UseMiddleware&lt;RequestLoggingMiddleware&gt;()</c>.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware, logging request details and elapsed time.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
