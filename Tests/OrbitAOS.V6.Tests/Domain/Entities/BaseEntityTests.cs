using FluentAssertions;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain.Entities
{
    /// <summary>
    /// Unit tests for BaseEntity – Domain layer.
    /// Tests default property values and property assignment.
    /// Note: BaseEntity is abstract; tested via concrete OrbitItem subclass.
    /// </summary>
    public class BaseEntityTests
    {
        // ─── Default values ─────────────────────────────────────────────────────

        [Fact]
        public void BaseEntity_DefaultId_IsZero()
        {
            // Arrange & Act
            var entity = new OrbitItem();

            // Assert
            entity.Id.Should().Be(0);
        }

        [Fact]
        public void BaseEntity_CreatedAt_DefaultsToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var entity = new OrbitItem();

            // Assert
            entity.CreatedAt.Should().BeAfter(before);
            entity.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
        }

        [Fact]
        public void BaseEntity_UpdatedAt_DefaultsToNull()
        {
            // Arrange & Act
            var entity = new OrbitItem();

            // Assert
            entity.UpdatedAt.Should().BeNull();
        }

        // ─── Property assignment ────────────────────────────────────────────────

        [Fact]
        public void BaseEntity_Id_CanBeSetAndRetrieved()
        {
            // Arrange
            var entity = new OrbitItem();

            // Act
            entity.Id = 42;

            // Assert
            entity.Id.Should().Be(42);
        }

        [Fact]
        public void BaseEntity_CreatedAt_CanBeOverridden()
        {
            // Arrange
            var customDate = new DateTime(2023, 6, 15, 10, 30, 0, DateTimeKind.Utc);
            var entity = new OrbitItem();

            // Act
            entity.CreatedAt = customDate;

            // Assert
            entity.CreatedAt.Should().Be(customDate);
        }

        [Fact]
        public void BaseEntity_UpdatedAt_CanBeSet()
        {
            // Arrange
            var updateTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var entity = new OrbitItem();

            // Act
            entity.UpdatedAt = updateTime;

            // Assert
            entity.UpdatedAt.Should().Be(updateTime);
        }

        [Fact]
        public void BaseEntity_UpdatedAt_CanBeSetToNull()
        {
            // Arrange
            var entity = new OrbitItem
            {
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            entity.UpdatedAt = null;

            // Assert
            entity.UpdatedAt.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(int.MaxValue)]
        public void BaseEntity_Id_AcceptsVariousPositiveValues(int id)
        {
            // Arrange
            var entity = new OrbitItem();

            // Act
            entity.Id = id;

            // Assert
            entity.Id.Should().Be(id);
        }
    }
}
