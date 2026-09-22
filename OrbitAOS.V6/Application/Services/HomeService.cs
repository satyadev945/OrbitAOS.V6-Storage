using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Application.Services
{
    /// <summary>
    /// Home service implementation – Application layer.
    /// Replaces inline logic that was previously embedded in legacy controllers.
    /// </summary>
    public class HomeService : IHomeService
    {
        private readonly ILogger<HomeService> _logger;

        public HomeService(ILogger<HomeService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<string> GetWelcomeMessageAsync()
        {
            _logger.LogInformation("GetWelcomeMessageAsync called at {Time}", DateTime.UtcNow);
            return Task.FromResult("Welcome to OrbitAOS – migrated to .NET 8 ASP.NET Core MVC.");
        }

        /// <inheritdoc />
        public Task<bool> IsHealthyAsync()
        {
            // In a real application, check database connectivity, external services, etc.
            return Task.FromResult(true);
        }
    }
}
