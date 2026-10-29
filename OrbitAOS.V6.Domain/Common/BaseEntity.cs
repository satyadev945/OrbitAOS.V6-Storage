namespace OrbitAOS.V6.Domain.Common
{
    /// <summary>
    /// Base entity class for all domain entities
    /// </summary>
    public abstract class BaseEntity : IAuditableEntity, ISoftDeletable
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
}
