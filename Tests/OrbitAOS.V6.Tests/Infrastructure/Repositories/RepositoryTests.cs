using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// Unit tests for the generic Repository using EF Core InMemory provider.
    /// Tests all CRUD operations without requiring a real SQL Server database.
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

        // ─── Constructor ────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidContext_CreatesInstance()
        {
            // Arrange & Act
            var repo = new Repository<OrbitItem>(_context);

            // Assert
            repo.Should().NotBeNull();
        }

        // ─── AddAsync ───────────────────────────────────────────────────────────

        [Fact]
        public async Task AddAsync_AddsEntityToDatabase()
        {
            // Arrange
            var item = new OrbitItem { Title = "Test Item" };

            // Act
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            // Assert
            var items = await _repository.GetAllAsync();
            items.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddAsync_EntityHasIdAfterSave()
        {
            // Arrange
            var item = new OrbitItem { Title = "New Item" };

            // Act
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            // Assert
            item.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddAsync_MultipleEntities_AllPersisted()
        {
            // Arrange
            var item1 = new OrbitItem { Title = "Item 1" };
            var item2 = new OrbitItem { Title = "Item 2" };
            var item3 = new OrbitItem { Title = "Item 3" };

            // Act
            await _repository.AddAsync(item1);
            await _repository.AddAsync(item2);
            await _repository.AddAsync(item3);
            await _repository.SaveChangesAsync();

            // Assert
            var items = await _repository.GetAllAsync();
            items.Should().HaveCount(3);
        }

        // ─── GetAllAsync ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyCollection()
        {
            // Act
            var items = await _repository.GetAllAsync();

            // Assert
            items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAsync_WithEntities_ReturnsAllEntities()
        {
            // Arrange
            await _repository.AddAsync(new OrbitItem { Title = "Item A" });
            await _repository.AddAsync(new OrbitItem { Title = "Item B" });
            await _repository.SaveChangesAsync();

            // Act
            var items = await _repository.GetAllAsync();

            // Assert
            items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCorrectTitles()
        {
            // Arrange
            await _repository.AddAsync(new OrbitItem { Title = "Alpha" });
            await _repository.AddAsync(new OrbitItem { Title = "Beta" });
            await _repository.SaveChangesAsync();

            // Act
            var items = (await _repository.GetAllAsync()).ToList();

            // Assert
            items.Select(i => i.Title).Should().Contain("Alpha");
            items.Select(i => i.Title).Should().Contain("Beta");
        }

        // ─── GetByIdAsync ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsEntity()
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
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var found = await _repository.GetByIdAsync(9999);

            // Assert
            found.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ZeroId_ReturnsNull()
        {
            // Act
            var found = await _repository.GetByIdAsync(0);

            // Assert
            found.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_NegativeId_ReturnsNull()
        {
            // Act
            var found = await _repository.GetByIdAsync(-1);

            // Assert
            found.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectEntity_WhenMultipleExist()
        {
            // Arrange
            var item1 = new OrbitItem { Title = "First" };
            var item2 = new OrbitItem { Title = "Second" };
            await _repository.AddAsync(item1);
            await _repository.AddAsync(item2);
            await _repository.SaveChangesAsync();

            // Act
            var found = await _repository.GetByIdAsync(item2.Id);

            // Assert
            found.Should().NotBeNull();
            found!.Title.Should().Be("Second");
        }

        // ─── Update ─────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_ModifiesEntityInDatabase()
        {
            // Arrange
            var item = new OrbitItem { Title = "Original" };
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            // Act
            item.Title = "Updated";
            _repository.Update(item);
            await _repository.SaveChangesAsync();

            // Assert
            var updated = await _repository.GetByIdAsync(item.Id);
            updated!.Title.Should().Be("Updated");
        }

        [Fact]
        public async Task Update_ModifiesMultipleProperties()
        {
            // Arrange
            var item = new OrbitItem { Title = "Original", Description = "Old Desc", IsActive = true };
            await _repository.AddAsync(item);
            await _repository.SaveChangesAsync();

            // Act
            item.Title = "New Title";
            item.Description = "New Desc";
            item.IsActive = false;
            _repository.Update(item);
            await _repository.SaveChangesAsync();

            // Assert
            var updated = await _repository.GetByIdAsync(item.Id);
            updated!.Title.Should().Be("New Title");
            updated.Description.Should().Be("New Desc");
            updated.IsActive.Should().BeFalse();
        }

        // ─── Remove ─────────────────────────────────────────────────────────────

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
        public async Task Remove_OnlyDeletesSpecifiedEntity()
        {
            // Arrange
            var item1 = new OrbitItem { Title = "Keep Me" };
            var item2 = new OrbitItem { Title = "Delete Me" };
            await _repository.AddAsync(item1);
            await _repository.AddAsync(item2);
            await _repository.SaveChangesAsync();

            // Act
            _repository.Remove(item2);
            await _repository.SaveChangesAsync();

            // Assert
            var items = (await _repository.GetAllAsync()).ToList();
            items.Should().HaveCount(1);
            items[0].Title.Should().Be("Keep Me");
        }

        // ─── SaveChangesAsync ───────────────────────────────────────────────────

        [Fact]
        public async Task SaveChangesAsync_ReturnsNumberOfAffectedRows()
        {
            // Arrange
            var item = new OrbitItem { Title = "Save Test" };
            await _repository.AddAsync(item);

            // Act
            var affected = await _repository.SaveChangesAsync();

            // Assert
            affected.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SaveChangesAsync_WithNoChanges_ReturnsZero()
        {
            // Act
            var affected = await _repository.SaveChangesAsync();

            // Assert
            affected.Should().Be(0);
        }

        // ─── IDisposable ────────────────────────────────────────────────────────

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
