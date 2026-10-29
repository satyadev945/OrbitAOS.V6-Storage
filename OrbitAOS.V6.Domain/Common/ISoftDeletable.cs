namespace OrbitAOS.V6.Domain.Common
{
    /// <summary>
    /// Interface for soft-deletable entities
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedDate { get; set; }
        string? DeletedBy { get; set; }
    }
}
