// ============================================================
// Application/Services/HomeService.cs
// Application service implementation — Clean Architecture Application Layer
// ============================================================

using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Application.Services;

/// <summary>
/// Implements <see cref="IHomeService"/>.
/// Business logic for the Home feature lives here, keeping the controller thin.
/// </summary>
public sealed class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;

    public HomeService(ILogger<HomeService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("HomeService.GetWelcomeMessageAsync called");
        // In a real application this would query a repository or configuration
        return Task.FromResult("Welcome to OrbitAOS — running on .NET 8");
    }
}
