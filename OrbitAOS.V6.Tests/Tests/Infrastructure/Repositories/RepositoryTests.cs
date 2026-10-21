// =============================================================================
// Tests/Infrastructure/Repositories/RepositoryTests.cs
// xUnit Tests for Repository<T> class
// Uses a test-specific DbContext that registers SampleEntity
// =============================================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Domain.Entities;
using OrbitAOS.V6.Infrastructure.Repositories;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Infrastructure.Repositories
{
    // Concrete entity for repository testing
    public class SampleEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    // Test-specific DbContext that includes SampleEntity in the model
    public class TestDbContext : IdentityDbContext<IdentityUser>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<SampleEntity> SampleEntities => Set<SampleEntity>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<SampleEntity>().HasKey(e => e.Id);
        }
    }

    // Concrete repository using the test DbContext
    public class TestRepository<T> : IDisposable where T : class
    {
        private readonly TestDbContext _context;
        private readonly DbSet<T> _dbSet;

        public TestRepository(TestDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Remove(T entity) => _dbSet.Remove(entity);
        public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate) => await _dbSet.AnyAsync(predicate);
        public async Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

    public class RepositoryTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly TestRepository<SampleEntity> _repository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TestDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new TestRepository<SampleEntity>(_context);
        }

        [Fact]
        public void Repository_CanBeInstantiated()
        {
            // Assert
            Assert.NotNull(_repository);
        }

        [Fact]
        public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyCollection()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_SingleEntity_EntityIsAdded()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Test", Value = 1 };

            // Act
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();

            // Assert
            var all = await _repository.GetAllAsync();
            Assert.Single(all);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsEntity()
        {
            // Arrange
            var entity = new SampleEntity { Name = "FindMe", Value = 99 };
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();

            // Act
            var found = await _repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(found);
            Assert.Equal("FindMe", found.Name);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(9999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Update_ExistingEntity_UpdatesValues()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Original", Value = 1 };
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();

            // Act
            entity.Name = "Updated";
            _repository.Update(entity);
            await _repository.SaveAsync();

            // Assert
            var updated = await _repository.GetByIdAsync(entity.Id);
            Assert.NotNull(updated);
            Assert.Equal("Updated", updated.Name);
        }

        [Fact]
        public async Task Remove_ExistingEntity_RemovesFromDatabase()
        {
            // Arrange
            var entity = new SampleEntity { Name = "ToRemove", Value = 5 };
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();

            // Act
            _repository.Remove(entity);
            await _repository.SaveAsync();

            // Assert
            var result = await _repository.GetByIdAsync(entity.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task ExistsAsync_EntityExists_ReturnsTrue()
        {
            // Arrange
            var entity = new SampleEntity { Name = "Exists", Value = 10 };
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();

            // Act
            var exists = await _repository.ExistsAsync(e => e.Name == "Exists");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_EntityDoesNotExist_ReturnsFalse()
        {
            // Act
            var exists = await _repository.ExistsAsync(e => e.Name == "NonExistent");

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task FindAsync_MatchingPredicate_ReturnsMatchingEntities()
        {
            // Arrange
            await _repository.AddAsync(new SampleEntity { Name = "Alpha", Value = 1 });
            await _repository.AddAsync(new SampleEntity { Name = "Beta", Value = 2 });
            await _repository.AddAsync(new SampleEntity { Name = "Alpha2", Value = 3 });
            await _repository.SaveAsync();

            // Act
            var results = await _repository.FindAsync(e => e.Name.StartsWith("Alpha"));

            // Assert
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task FindAsync_NoMatchingPredicate_ReturnsEmptyCollection()
        {
            // Arrange
            await _repository.AddAsync(new SampleEntity { Name = "Gamma", Value = 1 });
            await _repository.SaveAsync();

            // Act
            var results = await _repository.FindAsync(e => e.Name == "NonExistent");

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetAllAsync_MultipleEntities_ReturnsAll()
        {
            // Arrange
            await _repository.AddAsync(new SampleEntity { Name = "One", Value = 1 });
            await _repository.AddAsync(new SampleEntity { Name = "Two", Value = 2 });
            await _repository.AddAsync(new SampleEntity { Name = "Three", Value = 3 });
            await _repository.SaveAsync();

            // Act
            var all = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(3, all.Count());
        }

        [Fact]
        public async Task AddAsync_EntityHasCorrectName_AfterSave()
        {
            // Arrange
            var entity = new SampleEntity { Name = "NameCheck", Value = 7 };

            // Act
            await _repository.AddAsync(entity);
            await _repository.SaveAsync();
            var retrieved = await _repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("NameCheck", retrieved.Name);
            Assert.Equal(7, retrieved.Value);
        }

        [Fact]
        public async Task ExistsAsync_WithValuePredicate_ReturnsTrue()
        {
            // Arrange
            await _repository.AddAsync(new SampleEntity { Name = "Val", Value = 42 });
            await _repository.SaveAsync();

            // Act
            var exists = await _repository.ExistsAsync(e => e.Value == 42);

            // Assert
            Assert.True(exists);
        }

        // -----------------------------------------------------------------------
        // Tests for the actual Repository<T> using ApplicationDbContext
        // (testing constructor and basic instantiation)
        // -----------------------------------------------------------------------

        [Fact]
        public void ActualRepository_CanBeInstantiated()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var ctx = new OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext(options);

            // Act - IdentityUser is registered in ApplicationDbContext
            var repo = new Repository<IdentityUser>(ctx);

            // Assert
            Assert.NotNull(repo);
        }

        [Fact]
        public async Task ActualRepository_GetAllAsync_ReturnsEmptyForIdentityUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var ctx = new OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext(options);
            ctx.Database.EnsureCreated();
            var repo = new Repository<IdentityUser>(ctx);

            // Act
            var result = await repo.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ActualRepository_AddAsync_IdentityUser_Succeeds()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var ctx = new OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext(options);
            ctx.Database.EnsureCreated();
            var repo = new Repository<IdentityUser>(ctx);

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "repotest@example.com",
                Email = "repotest@example.com",
                NormalizedUserName = "REPOTEST@EXAMPLE.COM",
                NormalizedEmail = "REPOTEST@EXAMPLE.COM"
            };

            // Act
            await repo.AddAsync(user);
            await ctx.SaveChangesAsync();
            var all = await repo.GetAllAsync();

            // Assert
            Assert.Single(all);
        }

        [Fact]
        public async Task ActualRepository_ExistsAsync_ReturnsFalseWhenEmpty()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var ctx = new OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext(options);
            ctx.Database.EnsureCreated();
            var repo = new Repository<IdentityUser>(ctx);

            // Act
            var exists = await repo.ExistsAsync(u => u.Email == "nobody@example.com");

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ActualRepository_FindAsync_ReturnsEmptyWhenNoMatch()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var ctx = new OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext(options);
            ctx.Database.EnsureCreated();
            var repo = new Repository<IdentityUser>(ctx);

            // Act
            var results = await repo.FindAsync(u => u.Email == "nobody@example.com");

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task ActualRepository_ExistsAsync_ReturnsFalseForNonExistentEmail()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var ctx = new OrbitAOS.V6.Infrastructure.Persistence.ApplicationDbContext(options);
            ctx.Database.EnsureCreated();
            var repo = new Repository<IdentityUser>(ctx);

            // Act
            var result = await repo.ExistsAsync(u => u.Email == "missing@example.com");

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}
