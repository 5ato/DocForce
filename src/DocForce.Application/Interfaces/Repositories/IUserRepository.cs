using DocForce.Domain.Entities;

namespace DocForce.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistByUsername(string Username, CancellationToken cancellationToken = default);
    Task<User?> GetByUsername(string Username, CancellationToken cancellationToken = default);
    Task<User?> GetById(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
