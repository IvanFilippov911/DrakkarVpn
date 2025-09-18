using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerRepository
{
    Task<Server?> GetAsync(ServerId id, CancellationToken ct);
    Task AddAsync(Server server, CancellationToken ct);
    IQueryable<Server> Query();
}