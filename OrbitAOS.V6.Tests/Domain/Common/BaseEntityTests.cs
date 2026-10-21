// ============================================================
// Tests/Domain/Common/BaseEntityTests.cs
// Unit tests for BaseEntity — xUnit
// ============================================================

using OrbitAOS.V6.Domain.Common;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain.Common;

/// <summary>
/// Concrete subclass used to instantiate the abstract BaseEntity.
/// </summary>
internal class ConcreteEntity : BaseEntity { }

public class BaseEntityTests
{
    // ── Constructor / Default Values ─────────────────────────────────────────

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
    public void BaseEntity_DefaultCreatedAt_IsRecentUtc()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var entity = new ConcreteEntity();

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.True(entity.CreatedAt >= before && entity.CreatedAt <= after,
            $"CreatedAt {entity.CreatedAt} should be between {before} and {after}");
    }

    [Fact]
    public void BaseEntity_DefaultUpdatedAt_IsNull()
    {
        // Arrange & Act
        var entity = new ConcreteEntity();

        // Assert
        Assert.Null(entity.UpdatedAt);
    }

    // ── Property Setters ─────────────────────────────────────────────────────

    [Fact]
    public void BaseEntity_SetId_Persists()
    {
        // Arrange
        var entity = new ConcreteEntity();

        // Act
        entity.Id = 42;

        // Assert
        Assert.Equal(42, entity.Id);
    }

    [Fact]
    public void BaseEntity_SetIsDeleted_Persists()
    {
        // Arrange
        var entity = new ConcreteEntity();

        // Act
        entity.IsDeleted = true;

        // Assert
        Assert.True(entity.IsDeleted);
    }

    [Fact]
    public void BaseEntity_SetUpdatedAt_Persists()
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
    public void BaseEntity_SetCreatedAt_Persists()
    {
        // Arrange
        var entity = new ConcreteEntity();
        var customDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        entity.CreatedAt = customDate;

        // Assert
        Assert.Equal(customDate, entity.CreatedAt);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void BaseEntity_SetId_VariousValues(int id)
    {
        // Arrange
        var entity = new ConcreteEntity();

        // Act
        entity.Id = id;

        // Assert
        Assert.Equal(id, entity.Id);
    }
}
