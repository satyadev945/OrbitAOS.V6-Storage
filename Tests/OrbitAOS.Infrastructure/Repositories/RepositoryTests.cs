using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.Infrastructure.Repositories
{
    /// <summary>
    /// Comprehensive unit tests for <see cref="Repository{T}"/> using EF Core InMemory provider.
    /// </summary>
    public class RepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Repository<UserProfile> _repository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new Repository<UserProfile>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        // ── Constructor Tests ────────────────────────────────────────────────

        [Fact]
        public void Constructor_WithValidContext_CreatesInstance()
        {
            // Assert
            Assert.NotNull(_repository);
        }

        // ── GetByIdAsync Tests ───────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsEntity_WhenExists()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-1", "User One", "one@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(profile.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(profile.Id, result.Id);
            Assert.Equal("User One", result.DisplayName);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithZeroId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(0);

            // Assert
            Assert.Null(result);
        }

        // ── GetAllAsync Tests ────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            await _context.UserProfiles.AddRangeAsync(
                CreateSampleProfile("uid-1", "User One", "one@example.com"),
                CreateSampleProfile("uid-2", "User Two", "two@example.com"),
                CreateSampleProfile("uid-3", "User Three", "three@example.com")
            );
            await _context.SaveChangesAsync();

            // Act
            var result = (await _repository.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyCollection_WhenNoEntities()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        // ── AddAsync Tests ───────────────────────────────────────────────────

        [Fact]
        public async Task AddAsync_AddsEntityAndReturnsIt()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-add", "Add User", "add@example.com");

            // Act
            var result = await _repository.AddAsync(profile);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Add User", result.DisplayName);
        }

        [Fact]
        public async Task AddAsync_PersistsEntityToDatabase()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-persist", "Persist User", "persist@example.com");

            // Act
            await _repository.AddAsync(profile);
            var fromDb = await _context.UserProfiles.FindAsync(profile.Id);

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal("Persist User", fromDb.DisplayName);
        }

        [Fact]
        public async Task AddAsync_MultipleEntities_AllPersisted()
        {
            // Arrange
            var p1 = CreateSampleProfile("uid-m1", "Multi One", "m1@example.com");
            var p2 = CreateSampleProfile("uid-m2", "Multi Two", "m2@example.com");

            // Act
            await _repository.AddAsync(p1);
            await _repository.AddAsync(p2);
            var all = (await _repository.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, all.Count);
        }

        // ── UpdateAsync Tests ────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_UpdatesEntityInDatabase()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-upd", "Original Name", "orig@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            // Act
            profile.DisplayName = "Updated Name";
            await _repository.UpdateAsync(profile);
            var fromDb = await _context.UserProfiles.FindAsync(profile.Id);

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal("Updated Name", fromDb.DisplayName);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesEmailInDatabase()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-email", "Email User", "old@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            // Act
            profile.Email = "new@example.com";
            await _repository.UpdateAsync(profile);
            var fromDb = await _context.UserProfiles.FindAsync(profile.Id);

            // Assert
            Assert.NotNull(fromDb);
            Assert.Equal("new@example.com", fromDb.Email);
        }

        // ── DeleteAsync Tests ────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_RemovesEntityFromDatabase()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-del", "Delete User", "del@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            var id = profile.Id;

            // Act
            await _repository.DeleteAsync(id);
            var fromDb = await _context.UserProfiles.FindAsync(id);

            // Assert
            Assert.Null(fromDb);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
        {
            // Act & Assert - should not throw
            await _repository.DeleteAsync(99999);
        }

        [Fact]
        public async Task DeleteAsync_ReducesCount()
        {
            // Arrange
            var p1 = CreateSampleProfile("uid-d1", "Del One", "d1@example.com");
            var p2 = CreateSampleProfile("uid-d2", "Del Two", "d2@example.com");
            await _context.UserProfiles.AddRangeAsync(p1, p2);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(p1.Id);
            var all = (await _repository.GetAllAsync()).ToList();

            // Assert
            Assert.Single(all);
            Assert.Equal("Del Two", all[0].DisplayName);
        }

        // ── ExistsAsync Tests ────────────────────────────────────────────────

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenEntityExists()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-ex", "Exists User", "ex@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(profile.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenEntityNotExists()
        {
            // Act
            var result = await _repository.ExistsAsync(99999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_AfterDeletion()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-exd", "ExDel User", "exd@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            await _repository.DeleteAsync(profile.Id);

            // Act
            var result = await _repository.ExistsAsync(profile.Id);

            // Assert
            Assert.False(result);
        }

        // ── Helper Methods ───────────────────────────────────────────────────

        private static UserProfile CreateSampleProfile(string identityUserId, string displayName, string email)
            => new()
            {
                IdentityUserId = identityUserId,
                DisplayName = displayName,
                Email = email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
    }
}
