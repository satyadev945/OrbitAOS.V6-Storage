namespace OrbitAOS.V6.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when an entity is not found
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityName, object key)
            : base($"Entity '{entityName}' with key '{key}' was not found.")
        {
        }

        public EntityNotFoundException(string message) : base(message)
        {
        }
    }
}
