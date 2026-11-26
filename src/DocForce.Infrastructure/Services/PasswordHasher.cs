using DocForce.Application.Interfaces.Services;

namespace DocForce.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private readonly int _workFactor = 12;

    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, _workFactor);

    public bool VerifyPassword(string password, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
