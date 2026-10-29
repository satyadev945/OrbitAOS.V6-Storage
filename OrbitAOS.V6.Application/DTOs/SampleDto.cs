namespace OrbitAOS.V6.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for Sample entity
    /// </summary>
    public class SampleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// DTO for creating a new Sample
    /// </summary>
    public class CreateSampleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing Sample
    /// </summary>
    public class UpdateSampleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
