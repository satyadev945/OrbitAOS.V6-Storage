// ============================================================
// Application/Services/HomeService.cs
// Clean Architecture – Application Layer
//
// Concrete implementation of IHomeService.
// Business logic lives here, not in the controller.
// ============================================================

using OrbitAOS.V6.Application.Interfaces;

namespace OrbitAOS.V6.Application.Services;

/// <summary>
/// Provides business logic for the Home feature.
/// </summary>
public sealed class HomeService : IHomeService
{
    private readonly ILogger<HomeService> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="HomeService"/>.
    /// </summary>
    /// <param name="logger">Logger injected by the DI container.</param>
    public HomeService(ILogger<HomeService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("HomeService.GetWelcomeMessageAsync called.");
        return Task.FromResult("Welcome to OrbitAOS – running on .NET 8 with Clean Architecture.");
    }
}
