namespace DocForce.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
