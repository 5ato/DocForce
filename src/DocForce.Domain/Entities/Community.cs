using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class Community : BaseAuditableEntity
{
    public string Name { get; } = string.Empty;
    public Guid OwnerId { get; }

    public User? Owner { get; }
    public IList<User> Members { get; } = [];
    public IList<Role> Roles { get; } = [];

    private Community() { }
}
