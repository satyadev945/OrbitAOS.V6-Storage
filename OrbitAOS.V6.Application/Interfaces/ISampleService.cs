using OrbitAOS.V6.Application.DTOs;

namespace OrbitAOS.V6.Application.Interfaces;

/// <summary>
/// Service interface for Sample business logic operations.
/// Abstracts the business layer from the presentation layer.
/// Implemented in SampleService in the Application layer.
/// </summary>
public interface ISampleService
{
    /// <summary>Retrieves all active samples.</summary>
    Task<IEnumerable<SampleDto>> GetAllSamplesAsync();

    /// <summary>Retrieves a sample by its ID. Returns null if not found.</summary>
    Task<SampleDto?> GetSampleByIdAsync(int id);

    /// <summary>Creates a new sample and returns the created DTO with assigned ID.</summary>
    Task<SampleDto> CreateSampleAsync(SampleDto dto);

    /// <summary>Updates an existing sample. Throws KeyNotFoundException if not found.</summary>
    Task UpdateSampleAsync(int id, SampleDto dto);

    /// <summary>Deletes a sample by ID. Throws KeyNotFoundException if not found.</summary>
    Task DeleteSampleAsync(int id);
}
