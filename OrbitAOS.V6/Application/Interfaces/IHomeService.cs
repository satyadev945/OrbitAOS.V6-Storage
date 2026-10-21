// ============================================================
// Application/Interfaces/IHomeService.cs
// Clean Architecture – Application Layer
//
// Service interface for the Home feature.
// Controllers depend on this interface, not the concrete class.
// ============================================================

namespace OrbitAOS.V6.Application.Interfaces;

/// <summary>
/// Defines the contract for the Home feature's business logic.
/// </summary>
public interface IHomeService
{
    /// <summary>
    /// Returns the welcome message displayed on the home page.
    /// </summary>
    Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default);
}
