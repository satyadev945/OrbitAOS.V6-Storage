using OrbitAOS.Domain.Common;
using Xunit;

namespace OrbitAOS.Domain.Common
{
    // Concrete subclass for testing the abstract BaseEntity
    internal class ConcreteEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Comprehensive unit tests for <see cref="BaseEntity"/>.
    /// </summary>
    public class BaseEntityTests
    {
        // ── Constructor / Default State Tests ───────────────────────────────

        [Fact]
        public void BaseEntity_DefaultConstructor_CreatesInstance()
        {
            // Act
            var entity = new ConcreteEntity();

            // Assert
            Assert.NotNull(entity);
        }

        [Fact]
        public void BaseEntity_DefaultId_IsZero()
        {
            // Act
            var entity = new ConcreteEntity();

            // Assert
            Assert.Equal(0, entity.Id);
        }

        [Fact]
        public void BaseEntity_DefaultCreatedAt_IsApproximatelyUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var entity = new ConcreteEntity();
            var after = DateTime.UtcNow;

            // Assert
            Assert.True(entity.CreatedAt >= before && entity.CreatedAt <= after);
        }

        [Fact]
        public void BaseEntity_DefaultUpdatedAt_IsNull()
        {
            // Act
            var entity = new ConcreteEntity();

            // Assert
            Assert.Null(entity.UpdatedAt);
        }

        // ── Property Setter Tests ────────────────────────────────────────────

        [Fact]
        public void Id_SetValue_ReturnsValue()
        {
            // Arrange
            var entity = new ConcreteEntity();

            // Act
            entity.Id = 100;

            // Assert
            Assert.Equal(100, entity.Id);
        }

        [Fact]
        public void Id_SetNegativeValue_ReturnsNegativeValue()
        {
            // Arrange
            var entity = new ConcreteEntity();

            // Act
            entity.Id = -5;

            // Assert
            Assert.Equal(-5, entity.Id);
        }

        [Fact]
        public void Id_SetMaxValue_ReturnsMaxValue()
        {
            // Arrange
            var entity = new ConcreteEntity();

            // Act
            entity.Id = int.MaxValue;

            // Assert
            Assert.Equal(int.MaxValue, entity.Id);
        }

        [Fact]
        public void CreatedAt_SetValue_ReturnsValue()
        {
            // Arrange
            var entity = new ConcreteEntity();
            var date = new DateTime(2023, 5, 20, 10, 30, 0, DateTimeKind.Utc);

            // Act
            entity.CreatedAt = date;

            // Assert
            Assert.Equal(date, entity.CreatedAt);
        }

        [Fact]
        public void UpdatedAt_SetValue_ReturnsValue()
        {
            // Arrange
            var entity = new ConcreteEntity();
            var date = new DateTime(2024, 8, 1, 14, 0, 0, DateTimeKind.Utc);

            // Act
            entity.UpdatedAt = date;

            // Assert
            Assert.Equal(date, entity.UpdatedAt);
        }

        [Fact]
        public void UpdatedAt_SetNull_ReturnsNull()
        {
            // Arrange
            var entity = new ConcreteEntity { UpdatedAt = DateTime.UtcNow };

            // Act
            entity.UpdatedAt = null;

            // Assert
            Assert.Null(entity.UpdatedAt);
        }

        // ── Inheritance Tests ────────────────────────────────────────────────

        [Fact]
        public void ConcreteEntity_InheritsFromBaseEntity()
        {
            // Act
            var entity = new ConcreteEntity();

            // Assert
            Assert.IsAssignableFrom<BaseEntity>(entity);
        }

        [Fact]
        public void ConcreteEntity_CanSetOwnProperties()
        {
            // Arrange
            var entity = new ConcreteEntity();

            // Act
            entity.Name = "Test Name";

            // Assert
            Assert.Equal("Test Name", entity.Name);
        }

        [Fact]
        public void ConcreteEntity_ObjectInitializer_SetsAllProperties()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var updatedAt = new DateTime(2024, 7, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var entity = new ConcreteEntity
            {
                Id = 42,
                Name = "Init Entity",
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            Assert.Equal(42, entity.Id);
            Assert.Equal("Init Entity", entity.Name);
            Assert.Equal(createdAt, entity.CreatedAt);
            Assert.Equal(updatedAt, entity.UpdatedAt);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(999)]
        [InlineData(int.MaxValue)]
        public void Id_Theory_VariousValues(int id)
        {
            // Arrange
            var entity = new ConcreteEntity();

            // Act
            entity.Id = id;

            // Assert
            Assert.Equal(id, entity.Id);
        }
    }
}
