using FluentAssertions;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain.Entities
{
    /// <summary>
    /// Unit tests for OrbitItem domain entity and OrbitItemDto.
    /// Covers entity defaults, property assignment, and DTO mapping.
    /// </summary>
    public class OrbitItemTests
    {
        // ─── OrbitItem defaults ─────────────────────────────────────────────────

        [Fact]
        public void OrbitItem_DefaultTitle_IsEmptyString()
        {
            // Arrange & Act
            var item = new OrbitItem();

            // Assert
            item.Title.Should().BeEmpty();
        }

        [Fact]
        public void OrbitItem_DefaultIsActive_IsTrue()
        {
            // Arrange & Act
            var item = new OrbitItem();

            // Assert
            item.IsActive.Should().BeTrue();
        }

        [Fact]
        public void OrbitItem_DefaultDescription_IsNull()
        {
            // Arrange & Act
            var item = new OrbitItem();

            // Assert
            item.Description.Should().BeNull();
        }

        [Fact]
        public void OrbitItem_DefaultCreatedBy_IsNull()
        {
            // Arrange & Act
            var item = new OrbitItem();

            // Assert
            item.CreatedBy.Should().BeNull();
        }

        [Fact]
        public void OrbitItem_CreatedAt_DefaultsToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var item = new OrbitItem();

            // Assert
            item.CreatedAt.Should().BeAfter(before);
            item.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
        }

        // ─── OrbitItem property assignment ──────────────────────────────────────

        [Fact]
        public void OrbitItem_Title_CanBeSetAndRetrieved()
        {
            // Arrange
            var item = new OrbitItem();

            // Act
            item.Title = "My Orbit Item";

            // Assert
            item.Title.Should().Be("My Orbit Item");
        }

        [Fact]
        public void OrbitItem_Description_CanBeSetAndRetrieved()
        {
            // Arrange
            var item = new OrbitItem();

            // Act
            item.Description = "A detailed description";

            // Assert
            item.Description.Should().Be("A detailed description");
        }

        [Fact]
        public void OrbitItem_IsActive_CanBeSetToFalse()
        {
            // Arrange
            var item = new OrbitItem();

            // Act
            item.IsActive = false;

            // Assert
            item.IsActive.Should().BeFalse();
        }

        [Fact]
        public void OrbitItem_CreatedBy_CanBeSetAndRetrieved()
        {
            // Arrange
            var item = new OrbitItem();

            // Act
            item.CreatedBy = "admin@example.com";

            // Assert
            item.CreatedBy.Should().Be("admin@example.com");
        }

        [Fact]
        public void OrbitItem_ObjectInitializer_SetsAllProperties()
        {
            // Arrange & Act
            var item = new OrbitItem
            {
                Id = 10,
                Title = "Test Title",
                Description = "Test Description",
                IsActive = false,
                CreatedBy = "user1"
            };

            // Assert
            item.Id.Should().Be(10);
            item.Title.Should().Be("Test Title");
            item.Description.Should().Be("Test Description");
            item.IsActive.Should().BeFalse();
            item.CreatedBy.Should().Be("user1");
        }

        // ─── OrbitItemDto.FromEntity mapping ────────────────────────────────────

        [Fact]
        public void OrbitItemDto_FromEntity_MapsIdCorrectly()
        {
            // Arrange
            var entity = new OrbitItem { Id = 99, Title = "T" };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.Id.Should().Be(99);
        }

        [Fact]
        public void OrbitItemDto_FromEntity_MapsTitleCorrectly()
        {
            // Arrange
            var entity = new OrbitItem { Title = "Orbit Title" };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.Title.Should().Be("Orbit Title");
        }

        [Fact]
        public void OrbitItemDto_FromEntity_MapsDescriptionCorrectly()
        {
            // Arrange
            var entity = new OrbitItem { Title = "T", Description = "Some description" };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.Description.Should().Be("Some description");
        }

        [Fact]
        public void OrbitItemDto_FromEntity_MapsNullDescriptionCorrectly()
        {
            // Arrange
            var entity = new OrbitItem { Title = "T", Description = null };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.Description.Should().BeNull();
        }

        [Fact]
        public void OrbitItemDto_FromEntity_MapsIsActiveCorrectly()
        {
            // Arrange
            var entity = new OrbitItem { Title = "T", IsActive = false };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.IsActive.Should().BeFalse();
        }

        [Fact]
        public void OrbitItemDto_FromEntity_MapsCreatedByCorrectly()
        {
            // Arrange
            var entity = new OrbitItem { Title = "T", CreatedBy = "creator" };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.CreatedBy.Should().Be("creator");
        }

        [Fact]
        public void OrbitItemDto_FromEntity_MapsCreatedAtCorrectly()
        {
            // Arrange
            var createdAt = new DateTime(2024, 3, 15, 12, 0, 0, DateTimeKind.Utc);
            var entity = new OrbitItem
            {
                Title = "T",
                CreatedAt = createdAt
            };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.CreatedAt.Should().Be(createdAt);
        }

        [Fact]
        public void OrbitItemDto_FromEntity_MapsAllPropertiesAtOnce()
        {
            // Arrange
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var entity = new OrbitItem
            {
                Id = 42,
                Title = "Full Mapping",
                Description = "Full Description",
                IsActive = true,
                CreatedBy = "admin",
                CreatedAt = createdAt
            };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.Id.Should().Be(42);
            dto.Title.Should().Be("Full Mapping");
            dto.Description.Should().Be("Full Description");
            dto.IsActive.Should().BeTrue();
            dto.CreatedBy.Should().Be("admin");
            dto.CreatedAt.Should().Be(createdAt);
        }

        // ─── OrbitItemDto default values ────────────────────────────────────────

        [Fact]
        public void OrbitItemDto_DefaultConstructor_TitleIsEmpty()
        {
            // Arrange & Act
            var dto = new OrbitItemDto();

            // Assert
            dto.Title.Should().BeEmpty();
        }

        [Fact]
        public void OrbitItemDto_DefaultConstructor_IsActiveIsFalse()
        {
            // Arrange & Act
            var dto = new OrbitItemDto();

            // Assert
            dto.IsActive.Should().BeFalse();
        }

        [Fact]
        public void OrbitItemDto_DefaultConstructor_DescriptionIsNull()
        {
            // Arrange & Act
            var dto = new OrbitItemDto();

            // Assert
            dto.Description.Should().BeNull();
        }
    }
}
