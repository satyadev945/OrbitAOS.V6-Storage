// =============================================================================
// Tests/Infrastructure/Services/UnitOfWorkTests.cs
// xUnit Tests for UnitOfWork class
// =============================================================================

using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Infrastructure.Persistence;
using OrbitAOS.V6.Infrastructure.Services;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Infrastructure.Services
{
    public class UnitOfWorkTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _unitOfWork = new UnitOfWork(_context);
        }

        [Fact]
        public void UnitOfWork_CanBeInstantiated()
        {
            // Assert
            Assert.NotNull(_unitOfWork);
        }

        [Fact]
        public async Task SaveChangesAsync_WithNoChanges_ReturnsZero()
        {
            // Act
            var result = await _unitOfWork.SaveChangesAsync();

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
                _unitOfWork.SaveChangesAsync(cts.Token));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void UnitOfWork_Dispose_DoesNotThrow()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var ctx = new ApplicationDbContext(options);
            var uow = new UnitOfWork(ctx);

            // Act & Assert
            var ex = Record.Exception(() => uow.Dispose());
            Assert.Null(ex);
        }

        [Fact]
        public void UnitOfWork_DisposeCalledTwice_DoesNotThrow()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var ctx = new ApplicationDbContext(options);
            var uow = new UnitOfWork(ctx);

            // Act & Assert
            var ex = Record.Exception(() =>
            {
                uow.Dispose();
                uow.Dispose();
            });
            Assert.Null(ex);
        }

        [Fact]
        public void UnitOfWork_ImplementsIUnitOfWork()
        {
            // Assert
            Assert.IsAssignableFrom<OrbitAOS.V6.Domain.Interfaces.IUnitOfWork>(_unitOfWork);
        }

        [Fact]
        public void UnitOfWork_ImplementsIDisposable()
        {
            // Assert
            Assert.IsAssignableFrom<IDisposable>(_unitOfWork);
        }

        [Fact]
        public async Task CommitTransactionAsync_WithoutBeginTransaction_ThrowsInvalidOperationException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _unitOfWork.CommitTransactionAsync());
        }

        [Fact]
        public async Task RollbackTransactionAsync_WithoutActiveTransaction_DoesNotThrow()
        {
            // Act & Assert - should not throw when no transaction is active
            var ex = await Record.ExceptionAsync(() =>
                _unitOfWork.RollbackTransactionAsync());

            Assert.Null(ex);
        }

        [Fact]
        public async Task SaveChangesAsync_DefaultCancellationToken_ReturnsZero()
        {
            // Act
            var result = await _unitOfWork.SaveChangesAsync(default);

            // Assert
            Assert.Equal(0, result);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
