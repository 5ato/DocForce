using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class Permission : BaseAuditableEntity
{
    public string Name { get; } = string.Empty;

    public IList<PermissionRole> PermissionRoles { get; } = [];

    private Permission() { }
}
