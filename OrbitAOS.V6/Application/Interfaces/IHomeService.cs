// ============================================================
// Application/Interfaces/IHomeService.cs
// Application layer service interface for Home feature.
// Clean Architecture: Application layer orchestrates use cases.
// ============================================================
namespace OrbitAOS.V6.Application.Interfaces;

/// <summary>
/// Service interface for the Home feature.
/// Application layer contracts are defined here; implementations
/// live in Application/Services/.
/// 
/// Migration note: In legacy ASP.NET MVC, business logic was
/// often embedded directly in controller action methods.
/// This interface extracts that logic into a testable service.
/// </summary>
public interface IHomeService
{
    /// <summary>
    /// Returns the welcome message for the home page.
    /// Demonstrates the service pattern replacing inline controller logic.
    /// </summary>
    Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default);
}
