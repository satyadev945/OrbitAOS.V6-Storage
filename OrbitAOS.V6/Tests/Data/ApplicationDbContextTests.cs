using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using Xunit;

namespace OrbitAOS.V6.Tests.Data
{
    public class ApplicationDbContextTests
    {
        [Fact]
        public void Constructor_WithValidOptions_CreatesInstance()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void ApplicationDbContext_InheritsFromIdentityDbContext()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.IsAssignableFrom<IdentityDbContext>(context);
        }

        [Fact]
        public void ApplicationDbContext_CanBeDisposed()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Act & Assert
            using (var context = new ApplicationDbContext(options))
            {
                Assert.NotNull(context);
            }
        }

        [Fact]
        public void ApplicationDbContext_HasDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase4")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.Database);
        }

        [Fact]
        public void ApplicationDbContext_CanSaveChanges()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase5")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);
            var result = context.SaveChanges();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ApplicationDbContext_CanSaveChangesAsync()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase6")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);
            var result = await context.SaveChangesAsync();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ApplicationDbContext_HasUsers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase7")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.Users);
        }

        [Fact]
        public void ApplicationDbContext_HasRoles()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase8")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.Roles);
        }

        [Fact]
        public void ApplicationDbContext_CanCreateMultipleInstances()
        {
            // Arrange
            var options1 = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase9")
                .Options;
            var options2 = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase10")
                .Options;

            // Act
            using var context1 = new ApplicationDbContext(options1);
            using var context2 = new ApplicationDbContext(options2);

            // Assert
            Assert.NotNull(context1);
            Assert.NotNull(context2);
            Assert.NotSame(context1, context2);
        }

        [Fact]
        public void ApplicationDbContext_OptionsAreSet()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase11")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.NotNull(context.Database);
            Assert.True(context.Database.IsInMemory());
        }

        [Fact]
        public void ApplicationDbContext_CanBeCreatedWithDifferentDatabases()
        {
            // Arrange
            var options1 = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Database1")
                .Options;
            var options2 = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Database2")
                .Options;

            // Act
            using var context1 = new ApplicationDbContext(options1);
            using var context2 = new ApplicationDbContext(options2);

            // Assert
            Assert.NotNull(context1);
            Assert.NotNull(context2);
        }

        [Fact]
        public void ApplicationDbContext_InheritsDbContextProperties()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase12")
                .Options;

            // Act
            using var context = new ApplicationDbContext(options);

            // Assert
            Assert.IsAssignableFrom<DbContext>(context);
        }
    }
}
