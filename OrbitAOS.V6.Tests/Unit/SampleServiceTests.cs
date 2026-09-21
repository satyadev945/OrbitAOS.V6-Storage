using AutoMapper;
using FluentAssertions;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Unit;

/// <summary>
/// Unit tests for SampleService.
/// Tests all business logic methods using Moq for dependency mocking.
/// Follows AAA (Arrange-Act-Assert) pattern.
/// </summary>
public class SampleServiceTests
{
    private readonly Mock<IRepository<SampleEntity>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly SampleService _sampleService;

    public SampleServiceTests()
    {
        _mockRepository = new Mock<IRepository<SampleEntity>>();
        _mockMapper = new Mock<IMapper>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _sampleService = new SampleService(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllSamplesAsync_ShouldReturnAllSamples()
    {
        // Arrange
        var entities = new List<SampleEntity>
        {
            new SampleEntity { Id = 1, Name = "Sample 1", Description = "Description 1", IsActive = true },
            new SampleEntity { Id = 2, Name = "Sample 2", Description = "Description 2", IsActive = true }
        };
        var dtos = new List<SampleDto>
        {
            new SampleDto { Id = 1, Name = "Sample 1", Description = "Description 1", IsActive = true },
            new SampleDto { Id = 2, Name = "Sample 2", Description = "Description 2", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mockMapper.Setup(m => m.Map<IEnumerable<SampleDto>>(entities)).Returns(dtos);

        // Act
        var result = await _sampleService.GetAllSamplesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetSampleByIdAsync_WhenExists_ShouldReturnSample()
    {
        // Arrange
        var entity = new SampleEntity { Id = 1, Name = "Sample 1", Description = "Description 1", IsActive = true };
        var dto = new SampleDto { Id = 1, Name = "Sample 1", Description = "Description 1", IsActive = true };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<SampleDto>(entity)).Returns(dto);

        // Act
        var result = await _sampleService.GetSampleByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Sample 1");
    }

    [Fact]
    public async Task GetSampleByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((SampleEntity?)null);

        // Act
        var result = await _sampleService.GetSampleByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateSampleAsync_ShouldCreateAndReturnSample()
    {
        // Arrange
        var dto = new SampleDto { Name = "New Sample", Description = "New Description", IsActive = true };
        var entity = new SampleEntity { Name = "New Sample", Description = "New Description", IsActive = true };
        var createdEntity = new SampleEntity { Id = 1, Name = "New Sample", Description = "New Description", IsActive = true };
        var createdDto = new SampleDto { Id = 1, Name = "New Sample", Description = "New Description", IsActive = true };

        _mockMapper.Setup(m => m.Map<SampleEntity>(dto)).Returns(entity);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<SampleEntity>())).ReturnsAsync(createdEntity);
        _mockMapper.Setup(m => m.Map<SampleDto>(createdEntity)).Returns(createdDto);

        // Act
        var result = await _sampleService.CreateSampleAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateSampleAsync_WhenExists_ShouldUpdate()
    {
        // Arrange
        var entity = new SampleEntity { Id = 1, Name = "Old Name", Description = "Old Description", IsActive = true };
        var dto = new SampleDto { Id = 1, Name = "Updated Name", Description = "Updated Description", IsActive = false };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map(dto, entity)).Returns(entity);

        // Act
        await _sampleService.UpdateSampleAsync(1, dto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(entity), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateSampleAsync_WhenNotExists_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var dto = new SampleDto { Id = 999, Name = "Updated Name", Description = "Updated Description", IsActive = false };
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((SampleEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sampleService.UpdateSampleAsync(999, dto));
    }

    [Fact]
    public async Task DeleteSampleAsync_ShouldDeleteAndSaveChanges()
    {
        // Arrange
        var id = 1;
        _mockRepository.Setup(r => r.ExistsAsync(id)).ReturnsAsync(true);

        // Act
        await _sampleService.DeleteSampleAsync(id);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteSampleAsync_WhenNotExists_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sampleService.DeleteSampleAsync(999));
    }

    [Fact]
    public async Task GetAllSamplesAsync_WhenNoSamples_ShouldReturnEmptyList()
    {
        // Arrange
        var entities = new List<SampleEntity>();
        var dtos = new List<SampleDto>();

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mockMapper.Setup(m => m.Map<IEnumerable<SampleDto>>(entities)).Returns(dtos);

        // Act
        var result = await _sampleService.GetAllSamplesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
