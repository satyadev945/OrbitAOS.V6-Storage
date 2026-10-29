namespace OrbitAOS.V6.Domain.Common
{
    /// <summary>
    /// Interface for auditable entities
    /// </summary>
    public interface IAuditableEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
        string? CreatedBy { get; set; }
        string? ModifiedBy { get; set; }
    }
}
