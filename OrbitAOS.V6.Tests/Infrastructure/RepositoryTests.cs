using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure
{
    /// <summary>
    /// Unit tests for the generic Repository using EF Core InMemory provider.
    /// Tests data access patterns without requiring a real SQL Server database.
    /// </summary>
    public class RepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<OrbitItem> _repository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new Repository<OrbitItem>(_context);
        }

        [Fact]
        public async Task AddAsync_AddsEntityToDatabase()
        {
            // Arrange
            var item = new OrbitItem { Title = "Test Item", Description = "Test Description" };

            // Act
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            // Assert
            var items = await _repository.GetAllAsync();
            items.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            await _repository.AddAsync(new OrbitItem { Title = "Item 1" });
            await _repository.AddAsync(new OrbitItem { Title = "Item 2" });
            await _repository.SaveChangesAsync();

            // Act
            var items = await _repository.GetAllAsync();

            // Assert
            items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectEntity()
        {
            // Arrange
            var item = new OrbitItem { Title = "Find Me" };
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            // Act
            var found = await _repository.GetByIdAsync(item.Id);

            // Assert
            found.Should().NotBeNull();
            found!.Title.Should().Be("Find Me");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Act
            var found = await _repository.GetByIdAsync(9999);

            // Assert
            found.Should().BeNull();
        }

        [Fact]
        public async Task Remove_DeletesEntityFromDatabase()
        {
            // Arrange
            var item = new OrbitItem { Title = "Delete Me" };
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            // Act
            _repository.Remove(item);
            await _repository.SaveChangesAsync();

            // Assert
            var items = await _repository.GetAllAsync();
            items.Should().BeEmpty();
        }

        [Fact]
        public async Task Update_ModifiesEntityInDatabase()
        {
            // Arrange
            var item = new OrbitItem { Title = "Original Title" };
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            // Act
            item.Title = "Updated Title";
            _repository.Update(item);
            await _repository.SaveChangesAsync();

            // Assert
            var updated = await _repository.GetByIdAsync(item.Id);
            updated!.Title.Should().Be("Updated Title");
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
