using DocForce.Domain.Common.Interfaces;

namespace DocForce.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
