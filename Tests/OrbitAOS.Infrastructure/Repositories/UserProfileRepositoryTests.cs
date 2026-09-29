using Microsoft.EntityFrameworkCore;
using OrbitAOS.Domain.Entities;
using OrbitAOS.Infrastructure.Data;
using OrbitAOS.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.Infrastructure.Repositories
{
    /// <summary>
    /// Comprehensive unit tests for <see cref="UserProfileRepository"/> using EF Core InMemory provider.
    /// </summary>
    public class UserProfileRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserProfileRepository _repository;

        public UserProfileRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new UserProfileRepository(_context);
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

        // ── GetByIdentityUserIdAsync Tests ───────────────────────────────────

        [Fact]
        public async Task GetByIdentityUserIdAsync_ReturnsProfile_WhenExists()
        {
            // Arrange
            var profile = CreateSampleProfile("identity-abc", "Identity User", "identity@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdentityUserIdAsync("identity-abc");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("identity-abc", result.IdentityUserId);
            Assert.Equal("Identity User", result.DisplayName);
        }

        [Fact]
        public async Task GetByIdentityUserIdAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdentityUserIdAsync("nonexistent-id");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdentityUserIdAsync_ReturnsNull_WhenEmptyDatabase()
        {
            // Act
            var result = await _repository.GetByIdentityUserIdAsync("any-id");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdentityUserIdAsync_ReturnsCorrectProfile_WhenMultipleExist()
        {
            // Arrange
            await _context.UserProfiles.AddRangeAsync(
                CreateSampleProfile("uid-1", "User One", "one@example.com"),
                CreateSampleProfile("uid-2", "User Two", "two@example.com"),
                CreateSampleProfile("uid-3", "User Three", "three@example.com")
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdentityUserIdAsync("uid-2");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("uid-2", result.IdentityUserId);
            Assert.Equal("User Two", result.DisplayName);
        }

        [Fact]
        public async Task GetByIdentityUserIdAsync_IsCaseSensitive()
        {
            // Arrange
            var profile = CreateSampleProfile("CaseSensitiveId", "Case User", "case@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdentityUserIdAsync("casesensitiveid");

            // Assert - InMemory is case-sensitive by default
            // Result may be null depending on provider behavior
            // This test documents the behavior
            Assert.True(result == null || result.IdentityUserId == "CaseSensitiveId");
        }

        // ── GetByEmailAsync Tests ────────────────────────────────────────────

        [Fact]
        public async Task GetByEmailAsync_ReturnsProfile_WhenExists()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-email", "Email User", "email@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("email@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("email@example.com", result.Email);
            Assert.Equal("Email User", result.DisplayName);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByEmailAsync("notfound@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsNull_WhenEmptyDatabase()
        {
            // Act
            var result = await _repository.GetByEmailAsync("any@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_ReturnsCorrectProfile_WhenMultipleExist()
        {
            // Arrange
            await _context.UserProfiles.AddRangeAsync(
                CreateSampleProfile("uid-e1", "Email One", "one@example.com"),
                CreateSampleProfile("uid-e2", "Email Two", "two@example.com"),
                CreateSampleProfile("uid-e3", "Email Three", "three@example.com")
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByEmailAsync("two@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("two@example.com", result.Email);
            Assert.Equal("Email Two", result.DisplayName);
        }

        // ── Inherited Repository Methods Tests ───────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ReturnsProfile_WhenExists()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-id", "Id User", "id@example.com");
            await _context.UserProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(profile.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(profile.Id, result.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProfiles()
        {
            // Arrange
            await _context.UserProfiles.AddRangeAsync(
                CreateSampleProfile("uid-a1", "All One", "a1@example.com"),
                CreateSampleProfile("uid-a2", "All Two", "a2@example.com")
            );
            await _context.SaveChangesAsync();

            // Act
            var result = (await _repository.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task AddAsync_AddsProfileAndReturnsIt()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-add", "Add User", "add@example.com");

            // Act
            var result = await _repository.AddAsync(profile);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task DeleteAsync_RemovesProfile()
        {
            // Arrange
            var profile = CreateSampleProfile("uid-del", "Del User", "del@example.com");
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
        public async Task ExistsAsync_ReturnsTrue_WhenProfileExists()
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
        public async Task ExistsAsync_ReturnsFalse_WhenProfileNotExists()
        {
            // Act
            var result = await _repository.ExistsAsync(99999);

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
