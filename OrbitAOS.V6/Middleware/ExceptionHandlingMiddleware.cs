// =============================================================================
// Middleware/ExceptionHandlingMiddleware.cs
// Clean Architecture: Web Layer (Presentation) - Custom Middleware
//
// Migration Notes:
//   - Replaces legacy Global.asax Application_Error event handler
//   - Replaces legacy IHttpModule implementations (HTTP Modules)
//   - ASP.NET Core middleware pipeline replaces HTTP Modules/Handlers
//   - Provides centralized exception handling and logging
//   - Returns structured error responses (JSON for API, redirect for MVC)
// =============================================================================

using System.Net;
using System.Text.Json;

namespace OrbitAOS.V6.Middleware
{
    /// <summary>
    /// Custom exception handling middleware.
    /// Migration: Replaces Global.asax Application_Error and IHttpModule error handlers.
    /// 
    /// Legacy pattern (Global.asax):
    ///   protected void Application_Error(object sender, EventArgs e)
    ///   {
    ///       var exception = Server.GetLastError();
    ///       // handle exception
    ///   }
    /// 
    /// .NET 7 pattern: Middleware in the pipeline.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred. RequestId: {RequestId}, Path: {Path}",
                    context.TraceIdentifier, context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = exception switch
            {
                ArgumentNullException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            // For API requests, return JSON error response
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = _environment.IsDevelopment()
                        ? exception.Message
                        : "An internal server error occurred.",
                    Detail = _environment.IsDevelopment() ? exception.StackTrace : null
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            else
            {
                // For MVC requests, redirect to error page
                context.Response.Redirect($"/Home/Error");
            }
        }
    }

    /// <summary>
    /// Extension method for registering ExceptionHandlingMiddleware.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the custom exception handling middleware to the pipeline.
        /// Call this in Program.cs before other middleware.
        /// </summary>
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
