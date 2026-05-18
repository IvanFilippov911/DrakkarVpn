using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;
using DrakkarVpn.Servers.Domain.Aggregates;

namespace DrakkarVpn.Servers.Application.Abstractions.Repositories;

public interface IServerRepository
{
    Task<Server?> GetAsync(Guid id, CancellationToken ct);

    Task<Server?> GetForTransportActivationUpdateAsync(Guid serverId, CancellationToken ct);
    Task<Dictionary<Guid, Server>> GetByIdsAsync(Guid[] ids, CancellationToken ct);
    Task<Dictionary<Guid, Server>> GetWithActivationsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken ct);
    Task AddAsync(Server server, CancellationToken ct);
    IQueryable<Server> Query();
    Task DeleteAsync(Server server, CancellationToken ct);

    Task<(IReadOnlyList<AdminServerIndexRowDto> Items, int Total)> GetPagedAsync(
        string? region,
        ServerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct);

    Task<ServerForAgentDto?> GetForAgentAsync(Guid serverId, CancellationToken ct);
    Task<ServerShortDto?> GetServerShortAsync(Guid serverId, CancellationToken ct);
    
    Task<string?> GetAgentBaseUrlAsync(Guid serverId, CancellationToken ct);
}