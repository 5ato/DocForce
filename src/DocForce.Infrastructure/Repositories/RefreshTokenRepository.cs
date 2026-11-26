using DocForce.Application.Interfaces.Repositories;
using DocForce.Domain.Entities;
using DocForce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DocForce.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationContext _context;

    public RefreshTokenRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> GetByToken(string token, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
    }
}
