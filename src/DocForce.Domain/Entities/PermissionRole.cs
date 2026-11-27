using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class PermissionRole : BaseAuditableEntity
{
    public Guid PermissionId { get; }
    public Guid RoleId { get; }

    public Permission? Permission { get;}
    public Role? Role { get; }

    private PermissionRole() { }
}
