using OrbitAOS.V6.Application.DTOs;

namespace OrbitAOS.V6.Application.Interfaces
{
    /// <summary>
    /// Sample service interface - replace with your actual business logic interfaces
    /// </summary>
    public interface ISampleService
    {
        Task<IEnumerable<SampleDto>> GetAllSamplesAsync();
        Task<SampleDto?> GetSampleByIdAsync(int id);
        Task<SampleDto> CreateSampleAsync(CreateSampleDto dto);
        Task UpdateSampleAsync(int id, UpdateSampleDto dto);
        Task DeleteSampleAsync(int id);
    }
}
