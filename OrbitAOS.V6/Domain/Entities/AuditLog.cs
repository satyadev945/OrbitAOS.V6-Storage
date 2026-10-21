// ============================================================
// Domain/Entities/AuditLog.cs
// Example domain entity — Clean Architecture Domain Layer
// ============================================================

using OrbitAOS.V6.Domain.Common;

namespace OrbitAOS.V6.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking user actions.
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>The user who performed the action.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>The action performed (e.g., "Login", "Update", "Delete").</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>The entity type affected.</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>The entity identifier affected.</summary>
    public string? EntityId { get; set; }

    /// <summary>Additional details or JSON payload.</summary>
    public string? Details { get; set; }

    /// <summary>IP address of the request.</summary>
    public string? IpAddress { get; set; }

    /// <summary>UTC timestamp of the action.</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
