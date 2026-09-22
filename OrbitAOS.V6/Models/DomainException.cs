namespace OrbitAOS.V6.Models
{
    /// <summary>
    /// Represents a domain-level exception for business rule violations.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }

        public DomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
