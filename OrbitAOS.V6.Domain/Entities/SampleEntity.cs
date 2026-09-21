using OrbitAOS.V6.Domain.Common;

namespace OrbitAOS.V6.Domain.Entities;

/// <summary>
/// Sample entity demonstrating the domain model pattern.
/// Inherits audit properties from BaseEntity.
/// Used for CRUD demonstration in the application.
/// </summary>
public class SampleEntity : BaseEntity
{
    /// <summary>Display name of the sample (required, max 200 chars).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional description of the sample (max 1000 chars).</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Indicates whether the sample is currently active.</summary>
    public bool IsActive { get; set; } = true;
}
