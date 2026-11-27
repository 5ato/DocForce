using DocForce.Domain.Common;

namespace DocForce.Domain.Entities;

public class RefreshToken : BaseAuditableEntity
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsRevoked { get; private set; }

    public User? User { get; set; }

    private RefreshToken() { }
    public RefreshToken(string token, DateTime expiresAt, Guid userId)
    {
        Id = Guid.CreateVersion7();

        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
        IsRevoked = false;
    }

    public void RevokeToken()
    {
        IsRevoked = true;
    }
}
