namespace OrbitAOS.V6.Infrastructure.Services
{
    /// <summary>
    /// Interface for DateTime service to enable testability
    /// </summary>
    public interface IDateTime
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }

    /// <summary>
    /// Implementation of DateTime service
    /// </summary>
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
