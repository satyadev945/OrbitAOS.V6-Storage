// ============================================================
// Application/Services/HomeService.cs
// Application layer service implementation for Home feature.
// Implements IHomeService from Application/Interfaces/.
// ============================================================
using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Application.Services;

/// <summary>
/// Home feature service implementation.
/// Contains business logic that was previously embedded in
/// HomeController action methods in the legacy ASP.NET MVC app.
/// 
/// Migration note: Extracting logic from controllers into services
/// enables unit testing without spinning up the full MVC pipeline.
/// </summary>
public class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;

    public HomeService(ILogger<HomeService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("HomeService.GetWelcomeMessageAsync called at {Time}", DateTime.UtcNow);

        // Business logic lives here, not in the controller.
        // In a real application this might query a CMS, feature flags, etc.
        const string message = "Welcome to OrbitAOS - migrated to .NET 8 ASP.NET Core MVC.";
        return Task.FromResult(message);
    }
}
