using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Data
{
    /// <summary>
    /// Unit tests for ApplicationDbContext – Data layer.
    /// Tests DbContext configuration, entity sets, and model creation using InMemory provider.
    /// </summary>
    public class ApplicationDbContextTests : IDisposable
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
        }

        // ─── Constructor ────────────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidOptions_CreatesInstance()
        {
            // Assert
            _context.Should().NotBeNull();
        }

        // ─── OrbitItems DbSet ────────────────────────────────────────────────────

        [Fact]
        public void OrbitItems_DbSet_IsNotNull()
        {
            // Assert
            _context.OrbitItems.Should().NotBeNull();
        }

        [Fact]
        public async Task OrbitItems_InitiallyEmpty()
        {
            // Act
            var items = await _context.OrbitItems.ToListAsync();

            // Assert
            items.Should().BeEmpty();
        }

        [Fact]
        public async Task OrbitItems_CanAddAndRetrieveEntity()
        {
            // Arrange
            var item = new OrbitItem
            {
                Title = "Context Test Item",
                Description = "Test Description",
                IsActive = true,
                CreatedBy = "test-user"
            };

            // Act
            _context.OrbitItems.Add(item);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.OrbitItems.FindAsync(item.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Title.Should().Be("Context Test Item");
        }

        [Fact]
        public async Task OrbitItems_CanAddMultipleEntities()
        {
            // Arrange
            var items = new[]
            {
                new OrbitItem { Title = "Item 1" },
                new OrbitItem { Title = "Item 2" },
                new OrbitItem { Title = "Item 3" }
            };

            // Act
            _context.OrbitItems.AddRange(items);
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.OrbitItems.CountAsync();
            count.Should().Be(3);
        }

        [Fact]
        public async Task OrbitItems_CanUpdateEntity()
        {
            // Arrange
            var item = new OrbitItem { Title = "Original Title" };
            _context.OrbitItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            item.Title = "Updated Title";
            _context.OrbitItems.Update(item);
            await _context.SaveChangesAsync();

            // Assert
            var updated = await _context.OrbitItems.FindAsync(item.Id);
            updated!.Title.Should().Be("Updated Title");
        }

        [Fact]
        public async Task OrbitItems_CanRemoveEntity()
        {
            // Arrange
            var item = new OrbitItem { Title = "To Delete" };
            _context.OrbitItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            _context.OrbitItems.Remove(item);
            await _context.SaveChangesAsync();

            // Assert
            var count = await _context.OrbitItems.CountAsync();
            count.Should().Be(0);
        }

        // ─── SaveChangesAsync ────────────────────────────────────────────────────

        [Fact]
        public async Task SaveChangesAsync_WithNewEntity_ReturnsPositiveCount()
        {
            // Arrange
            _context.OrbitItems.Add(new OrbitItem { Title = "Save Test" });

            // Act
            var result = await _context.SaveChangesAsync();

            // Assert
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task SaveChangesAsync_WithNoChanges_ReturnsZero()
        {
            // Act
            var result = await _context.SaveChangesAsync();

            // Assert
            result.Should().Be(0);
        }

        // ─── Entity properties ───────────────────────────────────────────────────

        [Fact]
        public async Task OrbitItem_IsActive_DefaultsToTrue_WhenSaved()
        {
            // Arrange
            var item = new OrbitItem { Title = "Active Test" };
            _context.OrbitItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            var retrieved = await _context.OrbitItems.FindAsync(item.Id);

            // Assert
            retrieved!.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task OrbitItem_NullableDescription_CanBeNull()
        {
            // Arrange
            var item = new OrbitItem { Title = "No Description", Description = null };
            _context.OrbitItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            var retrieved = await _context.OrbitItems.FindAsync(item.Id);

            // Assert
            retrieved!.Description.Should().BeNull();
        }

        [Fact]
        public async Task OrbitItem_NullableCreatedBy_CanBeNull()
        {
            // Arrange
            var item = new OrbitItem { Title = "No Creator", CreatedBy = null };
            _context.OrbitItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            var retrieved = await _context.OrbitItems.FindAsync(item.Id);

            // Assert
            retrieved!.CreatedBy.Should().BeNull();
        }

        // ─── IDisposable ─────────────────────────────────────────────────────────

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
