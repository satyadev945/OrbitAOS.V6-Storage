using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Data;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Repositories
{
    /// <summary>
    /// Integration tests for Repository using InMemory database
    /// </summary>
    public class RepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<SampleEntity> _repository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new Repository<SampleEntity>(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntityToDatabase()
        {
            // Arrange
            var entity = new SampleEntity
            {
                Name = "Test Sample",
                Description = "Test Description",
                IsActive = true
            };

            // Act
            var result = await _repository.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Assert
            result.Should().NotBeNull();
            var savedEntity = await _context.SampleEntities.FirstOrDefaultAsync(e => e.Name == "Test Sample");
            savedEntity.Should().NotBeNull();
            savedEntity!.Name.Should().Be("Test Sample");
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnEntity()
        {
            // Arrange
            var entity = new SampleEntity
            {
                Name = "Test Sample",
                Description = "Test Description",
                IsActive = true
            };
            _context.SampleEntities.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(entity.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Test Sample");
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var entities = new List<SampleEntity>
            {
                new SampleEntity { Name = "Sample 1", Description = "Description 1", IsActive = true },
                new SampleEntity { Name = "Sample 2", Description = "Description 2", IsActive = true },
                new SampleEntity { Name = "Sample 3", Description = "Description 3", IsActive = false }
            };
            _context.SampleEntities.AddRange(entities);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            var entity = new SampleEntity
            {
                Name = "Original Name",
                Description = "Original Description",
                IsActive = true
            };
            _context.SampleEntities.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            entity.Name = "Updated Name";
            entity.Description = "Updated Description";
            await _repository.UpdateAsync(entity);
            await _context.SaveChangesAsync();

            // Assert
            var updatedEntity = await _context.SampleEntities.FindAsync(entity.Id);
            updatedEntity.Should().NotBeNull();
            updatedEntity!.Name.Should().Be("Updated Name");
            updatedEntity.Description.Should().Be("Updated Description");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            // Arrange
            var entity = new SampleEntity
            {
                Name = "Test Sample",
                Description = "Test Description",
                IsActive = true
            };
            _context.SampleEntities.Add(entity);
            await _context.SaveChangesAsync();
            var entityId = entity.Id;

            // Act
            await _repository.DeleteAsync(entityId);
            await _context.SaveChangesAsync();

            // Assert
            var deletedEntity = await _context.SampleEntities.FindAsync(entityId);
            deletedEntity.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var entity = new SampleEntity
            {
                Name = "Test Sample",
                Description = "Test Description",
                IsActive = true
            };
            _context.SampleEntities.Add(entity);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(entity.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.ExistsAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
