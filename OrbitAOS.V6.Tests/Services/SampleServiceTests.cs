using AutoMapper;
using FluentAssertions;
using Moq;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Services
{
    /// <summary>
    /// Unit tests for SampleService
    /// </summary>
    public class SampleServiceTests
    {
        private readonly Mock<IRepository<SampleEntity>> _mockRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SampleService _sampleService;

        public SampleServiceTests()
        {
            _mockRepository = new Mock<IRepository<SampleEntity>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _sampleService = new SampleService(_mockRepository.Object, _mockUnitOfWork.Object, _mockMapper.Object);
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
            result.Should().BeEquivalentTo(dtos);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSampleByIdAsync_WithValidId_ShouldReturnSample()
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
            result.Should().BeEquivalentTo(dto);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetSampleByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((SampleEntity?)null);

            // Act
            var result = await _sampleService.GetSampleByIdAsync(999);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        }

        [Fact]
        public async Task CreateSampleAsync_ShouldCreateAndReturnSample()
        {
            // Arrange
            var createDto = new CreateSampleDto { Name = "New Sample", Description = "New Description", IsActive = true };
            var entity = new SampleEntity { Name = "New Sample", Description = "New Description", IsActive = true };
            var createdEntity = new SampleEntity { Id = 1, Name = "New Sample", Description = "New Description", IsActive = true };
            var resultDto = new SampleDto { Id = 1, Name = "New Sample", Description = "New Description", IsActive = true };

            _mockMapper.Setup(m => m.Map<SampleEntity>(createDto)).Returns(entity);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<SampleEntity>())).ReturnsAsync(createdEntity);
            _mockMapper.Setup(m => m.Map<SampleDto>(createdEntity)).Returns(resultDto);

            // Act
            var result = await _sampleService.CreateSampleAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resultDto);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<SampleEntity>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateSampleAsync_WithValidId_ShouldUpdateSample()
        {
            // Arrange
            var updateDto = new UpdateSampleDto { Name = "Updated Sample", Description = "Updated Description", IsActive = false };
            var existingEntity = new SampleEntity { Id = 1, Name = "Old Sample", Description = "Old Description", IsActive = true };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingEntity);
            _mockMapper.Setup(m => m.Map(updateDto, existingEntity)).Returns(existingEntity);

            // Act
            await _sampleService.UpdateSampleAsync(1, updateDto);

            // Assert
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(existingEntity), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateSampleAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var updateDto = new UpdateSampleDto { Name = "Updated Sample", Description = "Updated Description", IsActive = false };
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((SampleEntity?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sampleService.UpdateSampleAsync(999, updateDto));
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<SampleEntity>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSampleAsync_WithValidId_ShouldDeleteSample()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);

            // Act
            await _sampleService.DeleteSampleAsync(1);

            // Assert
            _mockRepository.Verify(r => r.ExistsAsync(1), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteSampleAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            _mockRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sampleService.DeleteSampleAsync(999));
            _mockRepository.Verify(r => r.ExistsAsync(999), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
