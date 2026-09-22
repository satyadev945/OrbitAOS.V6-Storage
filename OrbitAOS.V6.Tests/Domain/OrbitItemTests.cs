using FluentAssertions;
using OrbitAOS.V6.Application.DTOs;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain
{
    /// <summary>
    /// Unit tests for domain entities and DTOs.
    /// </summary>
    public class OrbitItemTests
    {
        [Fact]
        public void OrbitItem_DefaultIsActive_IsTrue()
        {
            // Arrange & Act
            var item = new OrbitItem { Title = "Test" };

            // Assert
            item.IsActive.Should().BeTrue();
        }

        [Fact]
        public void OrbitItem_CreatedAt_IsSetToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var item = new OrbitItem { Title = "Test" };

            // Assert
            item.CreatedAt.Should().BeAfter(before);
            item.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
        }

        [Fact]
        public void OrbitItemDto_FromEntity_MapsAllProperties()
        {
            // Arrange
            var entity = new OrbitItem
            {
                Id = 42,
                Title = "Mapped Title",
                Description = "Mapped Description",
                IsActive = true,
                CreatedBy = "admin",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var dto = OrbitItemDto.FromEntity(entity);

            // Assert
            dto.Id.Should().Be(42);
            dto.Title.Should().Be("Mapped Title");
            dto.Description.Should().Be("Mapped Description");
            dto.IsActive.Should().BeTrue();
            dto.CreatedBy.Should().Be("admin");
            dto.CreatedAt.Should().Be(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
