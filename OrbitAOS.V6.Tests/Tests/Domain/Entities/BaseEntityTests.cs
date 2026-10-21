// =============================================================================
// Tests/Domain/Entities/BaseEntityTests.cs
// xUnit Tests for BaseEntity class
// =============================================================================

using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Tests.Domain.Entities
{
    // Concrete subclass for testing the abstract BaseEntity
    public class ConcreteEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class BaseEntityTests
    {
        [Fact]
        public void BaseEntity_DefaultId_IsZero()
        {
            // Arrange & Act
            var entity = new ConcreteEntity();

            // Assert
            Assert.Equal(0, entity.Id);
        }

        [Fact]
        public void BaseEntity_DefaultIsDeleted_IsFalse()
        {
            // Arrange & Act
            var entity = new ConcreteEntity();

            // Assert
            Assert.False(entity.IsDeleted);
        }

        [Fact]
        public void BaseEntity_DefaultCreatedAt_IsApproximatelyNow()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var entity = new ConcreteEntity();

            var after = DateTime.UtcNow.AddSeconds(1);

            // Assert
            Assert.True(entity.CreatedAt >= before && entity.CreatedAt <= after);
        }

        [Fact]
        public void BaseEntity_DefaultUpdatedAt_IsNull()
        {
            // Arrange & Act
            var entity = new ConcreteEntity();

            // Assert
            Assert.Null(entity.UpdatedAt);
        }

        [Fact]
        public void BaseEntity_SetId_ReturnsCorrectValue()
        {
            // Arrange
            var entity = new ConcreteEntity();

            // Act
            entity.Id = 42;

            // Assert
            Assert.Equal(42, entity.Id);
        }

        [Fact]
        public void BaseEntity_SetIsDeleted_ReturnsCorrectValue()
        {
            // Arrange
            var entity = new ConcreteEntity();

            // Act
            entity.IsDeleted = true;

            // Assert
            Assert.True(entity.IsDeleted);
        }

        [Fact]
        public void BaseEntity_SetUpdatedAt_ReturnsCorrectValue()
        {
            // Arrange
            var entity = new ConcreteEntity();
            var now = DateTime.UtcNow;

            // Act
            entity.UpdatedAt = now;

            // Assert
            Assert.Equal(now, entity.UpdatedAt);
        }

        [Fact]
        public void BaseEntity_SetCreatedAt_ReturnsCorrectValue()
        {
            // Arrange
            var entity = new ConcreteEntity();
            var specificDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            entity.CreatedAt = specificDate;

            // Assert
            Assert.Equal(specificDate, entity.CreatedAt);
        }

        [Fact]
        public void BaseEntity_MultipleInstances_HaveIndependentState()
        {
            // Arrange & Act
            var entity1 = new ConcreteEntity { Id = 1, IsDeleted = false };
            var entity2 = new ConcreteEntity { Id = 2, IsDeleted = true };

            // Assert
            Assert.NotEqual(entity1.Id, entity2.Id);
            Assert.NotEqual(entity1.IsDeleted, entity2.IsDeleted);
        }

        [Fact]
        public void ConcreteEntity_InheritsBaseEntityProperties()
        {
            // Arrange & Act
            var entity = new ConcreteEntity { Name = "Test" };

            // Assert
            Assert.IsAssignableFrom<BaseEntity>(entity);
            Assert.Equal("Test", entity.Name);
        }
    }
}
