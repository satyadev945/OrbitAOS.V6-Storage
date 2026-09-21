using System.ComponentModel.DataAnnotations;

namespace OrbitAOS.V6.Application.DTOs;

/// <summary>
/// Data Transfer Object for SampleEntity.
/// Used to transfer data between the Application and Web layers.
/// Decouples the domain model from the presentation layer.
/// </summary>
public class SampleDto
{
    /// <summary>Primary key identifier. Zero for new records.</summary>
    public int Id { get; set; }

    /// <summary>Display name of the sample.</summary>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional description of the sample.</summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Indicates whether the sample is currently active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>UTC timestamp when the sample was created.</summary>
    public DateTime CreatedDate { get; set; }
}
