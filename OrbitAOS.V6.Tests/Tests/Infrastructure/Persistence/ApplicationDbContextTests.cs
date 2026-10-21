// =============================================================================
// Tests/Infrastructure/Persistence/ApplicationDbContextTests.cs
// xUnit Tests for ApplicationDbContext class
// =============================================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Persistence;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Infrastructure.Persistence
{
    // Concrete entity for testing BaseEntity audit fields via DbContext
    public class TestAuditEntity : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
    }

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

        [Fact]
        public void ApplicationDbContext_CanBeInstantiated()
        {
            // Assert
            Assert.NotNull(_context);
        }

        [Fact]
        public void ApplicationDbContext_CanEnsureCreated()
        {
            // Act
            var created = _context.Database.EnsureCreated();

            // Assert
            Assert.True(created);
        }

        [Fact]
        public async Task SaveChangesAsync_WithNoChanges_ReturnsZero()
        {
            // Act
            var result = await _context.SaveChangesAsync(CancellationToken.None);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task SaveChangesAsync_WithCancellationToken_DoesNotThrow()
        {
            // Arrange
            using var cts = new CancellationTokenSource();

            // Act
            var ex = await Record.ExceptionAsync(() =>
                _context.SaveChangesAsync(cts.Token));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void ApplicationDbContext_ImplementsIApplicationDbContext()
        {
            // Assert
            Assert.IsAssignableFrom<OrbitAOS.V6.Application.Common.Interfaces.IApplicationDbContext>(_context);
        }

        [Fact]
        public void ApplicationDbContext_CanAccessIdentityUsers()
        {
            // Act
            var users = _context.Users;

            // Assert
            Assert.NotNull(users);
        }

        [Fact]
        public void ApplicationDbContext_CanAccessIdentityRoles()
        {
            // Act
            var roles = _context.Roles;

            // Assert
            Assert.NotNull(roles);
        }

        [Fact]
        public async Task SaveChangesAsync_WithIdentityUser_PersistsData()
        {
            // Arrange
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testuser@example.com",
                Email = "testuser@example.com",
                NormalizedUserName = "TESTUSER@EXAMPLE.COM",
                NormalizedEmail = "TESTUSER@EXAMPLE.COM"
            };

            // Act
            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync(CancellationToken.None);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task SaveChangesAsync_CanRetrievePersistedUser()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new IdentityUser
            {
                Id = userId,
                UserName = "retrieve@example.com",
                Email = "retrieve@example.com",
                NormalizedUserName = "RETRIEVE@EXAMPLE.COM",
                NormalizedEmail = "RETRIEVE@EXAMPLE.COM"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync(CancellationToken.None);

            // Act
            var retrieved = await _context.Users.FindAsync(userId);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("retrieve@example.com", retrieved.Email);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
