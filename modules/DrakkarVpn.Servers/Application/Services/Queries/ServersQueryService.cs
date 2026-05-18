using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using DrakkarVpn.Servers.Application.Mappers;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Servers.Application.Services.Queries;

public sealed class ServersQueryService : IServersQueryService
{
    private readonly IServerRepository _servers;

    public ServersQueryService(IServerRepository servers)
        => _servers = servers;

    public async Task<PagedResponseDto<GetServerDto>> GetPagedAsync(
        string? region,
        ServerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 50 : pageSize;
        pageSize = Math.Clamp(pageSize, 1, 200);

        var (rows, total) = await _servers.GetPagedAsync(
            region: region,
            status: status,
            page: page,
            pageSize: pageSize,
            ct: ct);

        if (total == 0 || rows.Count == 0)
            return PagedResponseDto<GetServerDto>.Empty(page, pageSize);

        var items = rows.Select(x => x.ToGetServerDto()).ToList();

        return PagedResponseDto<GetServerDto>.From(items, page, pageSize, total);
    }

    public async Task<GetServersDetailDto?> GetDetailAsync(Guid serverId, CancellationToken ct)
    {
        var s = await _servers.GetAsync(serverId, ct);
        if (s is null)
            return null;

        return new GetServersDetailDto(
            s.Id,
            s.Name,
            s.Region.Code,
            s.PublicHost.Value,
            s.AgentBaseUrl.ToString(),
            s.Status.ToString(),
            s.Health.Reachable,
            s.Health.PeersActive,
            s.MaxPeers,
            s.Metrics.TrafficRxBytes,
            s.Metrics.TrafficTxBytes,
            s.Metrics.VpnSpeedMbps,
            s.Metrics.InfraLatencyMs);
    }
}
