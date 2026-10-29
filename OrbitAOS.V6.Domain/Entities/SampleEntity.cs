using OrbitAOS.V6.Domain.Common;

namespace OrbitAOS.V6.Domain.Entities
{
    /// <summary>
    /// Sample entity - replace with your actual domain entities
    /// </summary>
    public class SampleEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
