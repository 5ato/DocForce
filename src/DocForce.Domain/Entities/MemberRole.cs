using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class MemberRole : BaseAuditableEntity
{
    public Guid MemberId { get; }
    public Guid RoleId { get; }

    public Member? Member { get; }
    public Role? Role { get; }

    private MemberRole() { }
}
