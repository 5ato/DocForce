using DocForce.Application.Interfaces.Repositories;
using DocForce.Domain.Entities;
using DocForce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace DocForce.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationContext _context;

    public UserRepository(ApplicationContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<bool> ExistByUsername(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByUsername(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
}