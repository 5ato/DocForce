using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Username { get; } = string.Empty;
    public string PasswordHash { get; } = string.Empty;

    public IList<RefreshToken> RefreshTokens { get; } = [];
    public IList<Community> OwnedCommunities { get; } = [];
    public IList<Member> Memberships { get; } = [];

    private User() { }
    public User(string username, string passwordHash)
    {
        Id = Guid.CreateVersion7();

        Username = username;
        PasswordHash = passwordHash;
    }
}
