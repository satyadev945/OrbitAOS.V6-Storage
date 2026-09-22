namespace OrbitAOS.V6.Application.Interfaces
{
    /// <summary>
    /// Application service interface for the Home feature.
    /// Application layer – orchestrates domain logic, no framework dependencies.
    /// </summary>
    public interface IHomeService
    {
        /// <summary>Returns a welcome message for the home page.</summary>
        Task<string> GetWelcomeMessageAsync();

        /// <summary>Returns application health status.</summary>
        Task<bool> IsHealthyAsync();
    }
}
