namespace DrakkarVpn.Servers.Application.Abstractions;

public interface IServersUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}