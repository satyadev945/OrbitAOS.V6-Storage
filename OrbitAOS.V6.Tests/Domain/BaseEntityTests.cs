// ============================================================
// Tests/Domain/BaseEntityTests.cs
// Unit Tests – Domain Layer
//
// Tests the BaseEntity abstract class properties and defaults.
// ============================================================

using FluentAssertions;
using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain;

// ---------------------------------------------------------------------------
// Concrete subclass for testing the abstract BaseEntity
// ---------------------------------------------------------------------------

/// <summary>
/// Concrete implementation of <see cref="BaseEntity"/> for testing purposes.
/// </summary>
public class ConcreteEntity : BaseEntity
{
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// Unit tests for <see cref="BaseEntity"/>.
/// </summary>
public class BaseEntityTests
{
    // ------------------------------------------------------------------
    // Constructor / Default values
    // ------------------------------------------------------------------

    [Fact]
    public void BaseEntity_NewInstance_HasNonEmptyGuidId()
    {
        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void BaseEntity_NewInstance_IdIsGuid()
    {
        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void BaseEntity_TwoInstances_HaveDifferentIds()
    {
        // Act
        var entity1 = new ConcreteEntity();
        var entity2 = new ConcreteEntity();

        // Assert
        entity1.Id.Should().NotBe(entity2.Id);
    }

    [Fact]
    public void BaseEntity_NewInstance_CreatedAtIsSetToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var entity = new ConcreteEntity();

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        entity.CreatedAt.Should().BeAfter(before).And.BeBefore(after);
    }

    [Fact]
    public void BaseEntity_NewInstance_UpdatedAtIsNull()
    {
        // Act
        var entity = new ConcreteEntity();

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    // ------------------------------------------------------------------
    // Property setters
    // ------------------------------------------------------------------

    [Fact]
    public void BaseEntity_SetId_ReturnsSetValue()
    {
        // Arrange
        var entity = new ConcreteEntity();
        var newId = Guid.NewGuid();

        // Act
        entity.Id = newId;

        // Assert
        entity.Id.Should().Be(newId);
    }

    [Fact]
    public void BaseEntity_SetCreatedAt_ReturnsSetValue()
    {
        // Arrange
        var entity = new ConcreteEntity();
        var timestamp = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        entity.CreatedAt = timestamp;

        // Assert
        entity.CreatedAt.Should().Be(timestamp);
    }

    [Fact]
    public void BaseEntity_SetUpdatedAt_ReturnsSetValue()
    {
        // Arrange
        var entity = new ConcreteEntity();
        var timestamp = new DateTime(2024, 6, 20, 12, 0, 0, DateTimeKind.Utc);

        // Act
        entity.UpdatedAt = timestamp;

        // Assert
        entity.UpdatedAt.Should().Be(timestamp);
    }

    [Fact]
    public void BaseEntity_SetUpdatedAtToNull_ReturnsNull()
    {
        // Arrange
        var entity = new ConcreteEntity
        {
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        entity.UpdatedAt = null;

        // Assert
        entity.UpdatedAt.Should().BeNull();
    }

    // ------------------------------------------------------------------
    // Subclass properties
    // ------------------------------------------------------------------

    [Fact]
    public void ConcreteEntity_InheritsBaseEntityProperties()
    {
        // Act
        var entity = new ConcreteEntity { Title = "Test" };

        // Assert
        entity.Id.Should().NotBe(Guid.Empty);
        entity.CreatedAt.Should().NotBe(default);
        entity.Title.Should().Be("Test");
    }
}
