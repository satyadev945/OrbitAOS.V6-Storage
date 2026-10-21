// ============================================================
// Application/Interfaces/IHomeService.cs
// Application service interface — Clean Architecture Application Layer
// ============================================================

namespace OrbitAOS.V6.Application.Interfaces;

/// <summary>
/// Contract for the Home feature service.
/// Keeps controllers thin by delegating business logic here.
/// </summary>
public interface IHomeService
{
    /// <summary>Returns the welcome message for the home page.</summary>
    Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default);
}
