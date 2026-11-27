using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class Member : BaseAuditableEntity
{
    public Guid UserId { get; }
    public Guid CommunityId { get; }

    public User? User { get; }
    public Community? Community { get; }
    public IList<MemberRole> MemberRoles { get; } = [];

    private Member() { }
}
