using DocForce.Application.DTOs.User;
using DocForce.Domain.Entities;
using System.Security.Claims;

namespace DocForce.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
}
