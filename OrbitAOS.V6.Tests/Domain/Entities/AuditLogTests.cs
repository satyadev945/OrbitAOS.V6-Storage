// ============================================================
// Tests/Domain/Entities/AuditLogTests.cs
// Unit tests for AuditLog entity — xUnit
// ============================================================

using OrbitAOS.V6.Domain.Entities;
using Xunit;

namespace OrbitAOS.V6.Tests.Domain.Entities;

public class AuditLogTests
{
    // ── Constructor / Default Values ─────────────────────────────────────────

    [Fact]
    public void AuditLog_DefaultUserId_IsEmptyString()
    {
        // Arrange & Act
        var log = new AuditLog();

        // Assert
        Assert.Equal(string.Empty, log.UserId);
    }

    [Fact]
    public void AuditLog_DefaultAction_IsEmptyString()
    {
        // Arrange & Act
        var log = new AuditLog();

        // Assert
        Assert.Equal(string.Empty, log.Action);
    }

    [Fact]
    public void AuditLog_DefaultEntityType_IsEmptyString()
    {
        // Arrange & Act
        var log = new AuditLog();

        // Assert
        Assert.Equal(string.Empty, log.EntityType);
    }

    [Fact]
    public void AuditLog_DefaultEntityId_IsNull()
    {
        // Arrange & Act
        var log = new AuditLog();

        // Assert
        Assert.Null(log.EntityId);
    }

    [Fact]
    public void AuditLog_DefaultDetails_IsNull()
    {
        // Arrange & Act
        var log = new AuditLog();

        // Assert
        Assert.Null(log.Details);
    }

    [Fact]
    public void AuditLog_DefaultIpAddress_IsNull()
    {
        // Arrange & Act
        var log = new AuditLog();

        // Assert
        Assert.Null(log.IpAddress);
    }

    [Fact]
    public void AuditLog_DefaultTimestamp_IsRecentUtc()
    {
        // Arrange
        var before = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var log = new AuditLog();

        // Assert
        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.True(log.Timestamp >= before && log.Timestamp <= after,
            $"Timestamp {log.Timestamp} should be between {before} and {after}");
    }

    // ── Property Setters ─────────────────────────────────────────────────────

    [Fact]
    public void AuditLog_SetUserId_Persists()
    {
        // Arrange
        var log = new AuditLog();

        // Act
        log.UserId = "user-123";

        // Assert
        Assert.Equal("user-123", log.UserId);
    }

    [Fact]
    public void AuditLog_SetAction_Persists()
    {
        // Arrange
        var log = new AuditLog();

        // Act
        log.Action = "Login";

        // Assert
        Assert.Equal("Login", log.Action);
    }

    [Fact]
    public void AuditLog_SetEntityType_Persists()
    {
        // Arrange
        var log = new AuditLog();

        // Act
        log.EntityType = "Order";

        // Assert
        Assert.Equal("Order", log.EntityType);
    }

    [Fact]
    public void AuditLog_SetEntityId_Persists()
    {
        // Arrange
        var log = new AuditLog();

        // Act
        log.EntityId = "42";

        // Assert
        Assert.Equal("42", log.EntityId);
    }

    [Fact]
    public void AuditLog_SetDetails_Persists()
    {
        // Arrange
        var log = new AuditLog();
        const string details = "{\"key\":\"value\"}";

        // Act
        log.Details = details;

        // Assert
        Assert.Equal(details, log.Details);
    }

    [Fact]
    public void AuditLog_SetIpAddress_Persists()
    {
        // Arrange
        var log = new AuditLog();

        // Act
        log.IpAddress = "192.168.1.1";

        // Assert
        Assert.Equal("192.168.1.1", log.IpAddress);
    }

    [Fact]
    public void AuditLog_SetTimestamp_Persists()
    {
        // Arrange
        var log = new AuditLog();
        var ts = new DateTime(2024, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        // Act
        log.Timestamp = ts;

        // Assert
        Assert.Equal(ts, log.Timestamp);
    }

    [Fact]
    public void AuditLog_InheritsBaseEntity_IdDefaultsToZero()
    {
        // Arrange & Act
        var log = new AuditLog();

        // Assert
        Assert.Equal(0, log.Id);
    }

    [Fact]
    public void AuditLog_InheritsBaseEntity_IsDeletedDefaultsFalse()
    {
        // Arrange & Act
        var log = new AuditLog();

        // Assert
        Assert.False(log.IsDeleted);
    }

    [Theory]
    [InlineData("Login")]
    [InlineData("Update")]
    [InlineData("Delete")]
    [InlineData("Create")]
    public void AuditLog_SetAction_VariousValues(string action)
    {
        // Arrange
        var log = new AuditLog();

        // Act
        log.Action = action;

        // Assert
        Assert.Equal(action, log.Action);
    }

    [Fact]
    public void AuditLog_FullyPopulated_AllPropertiesCorrect()
    {
        // Arrange & Act
        var ts = DateTime.UtcNow;
        var log = new AuditLog
        {
            Id = 1,
            UserId = "user-abc",
            Action = "Delete",
            EntityType = "Product",
            EntityId = "99",
            Details = "Deleted product 99",
            IpAddress = "10.0.0.1",
            Timestamp = ts,
            IsDeleted = false
        };

        // Assert
        Assert.Equal(1, log.Id);
        Assert.Equal("user-abc", log.UserId);
        Assert.Equal("Delete", log.Action);
        Assert.Equal("Product", log.EntityType);
        Assert.Equal("99", log.EntityId);
        Assert.Equal("Deleted product 99", log.Details);
        Assert.Equal("10.0.0.1", log.IpAddress);
        Assert.Equal(ts, log.Timestamp);
        Assert.False(log.IsDeleted);
    }
}
