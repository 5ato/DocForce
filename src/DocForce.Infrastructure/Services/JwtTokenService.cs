using DocForce.Application.Interfaces.Services;
using DocForce.Domain.Entities;
using DocForce.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DocForce.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOption _jwtOption;
    public JwtTokenService(IOptions<JwtOption> options)
    {
        _jwtOption = options.Value;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOption.Issuer,
            audience: _jwtOption.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOption.AccessTokenExpirationMinutes),
            signingCredentials: cred);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return new RefreshToken(
            Convert.ToBase64String(bytes),
            DateTime.UtcNow.AddDays(_jwtOption.RefreshTokenExpirationDays),
            userId);
    }
}
