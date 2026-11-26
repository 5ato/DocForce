using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public IList<RefreshToken> RefreshTokens { get; set; } = [];

    private User() { }
    public User(string username, string passwordHash)
    {
        Id = Guid.CreateVersion7();

        Username = username;
        PasswordHash = passwordHash;

        UpdatedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}
