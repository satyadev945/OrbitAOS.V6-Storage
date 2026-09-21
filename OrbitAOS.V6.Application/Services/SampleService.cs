using AutoMapper;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Domain.Entities;

namespace OrbitAOS.V6.Application.Services;

/// <summary>
/// Sample service implementation containing business logic for Sample CRUD operations.
/// Uses Repository pattern and Unit of Work for data access.
/// Uses AutoMapper for entity-DTO mapping.
/// Replaces business logic that was previously embedded in legacy MVC controllers.
/// </summary>
public class SampleService : ISampleService
{
    private readonly IRepository<SampleEntity> _repository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public SampleService(
        IRepository<SampleEntity> repository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SampleDto>> GetAllSamplesAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<SampleDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<SampleDto?> GetSampleByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity != null ? _mapper.Map<SampleDto>(entity) : null;
    }

    /// <inheritdoc />
    public async Task<SampleDto> CreateSampleAsync(SampleDto dto)
    {
        var entity = _mapper.Map<SampleEntity>(dto);
        entity.CreatedDate = DateTime.UtcNow;
        entity.IsDeleted = false;

        var created = await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<SampleDto>(created);
    }

    /// <inheritdoc />
    public async Task UpdateSampleAsync(int id, SampleDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Sample with ID {id} not found.");

        _mapper.Map(dto, entity);
        entity.ModifiedDate = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task DeleteSampleAsync(int id)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"Sample with ID {id} not found.");

        await _repository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
