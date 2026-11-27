using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public Guid CommunityId { get; }
    public string Name { get; } = string.Empty;

    public Community? Community { get; }
    public IList<MemberRole> MemberRoles { get; } = [];
    public IList<PermissionRole> PermissionRoles { get; } = [];

    private Role() { }
}
