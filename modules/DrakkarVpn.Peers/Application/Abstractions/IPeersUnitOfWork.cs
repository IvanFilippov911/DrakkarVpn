namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeersUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}