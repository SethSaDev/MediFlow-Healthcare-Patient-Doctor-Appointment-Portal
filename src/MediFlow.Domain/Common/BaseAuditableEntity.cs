namespace MediFlow.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }
}