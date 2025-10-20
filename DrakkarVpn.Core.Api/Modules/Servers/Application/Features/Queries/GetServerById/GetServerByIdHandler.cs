using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerById;

public sealed class GetServerByIdHandler : IRequestHandler<GetServerByIdRequest, GetServersDetailDto?>
{
    private readonly IServerRepository _repo;
    public GetServerByIdHandler(IServerRepository repo) => _repo = repo;

    public async Task<GetServersDetailDto?> Handle(GetServerByIdRequest request, CancellationToken ct)
    {
        var server = await _repo.GetAsync(new ServerId(request.ServerId), ct);
        if (server is null) return null;

        return new GetServersDetailDto(
            server.Id.Value,
            server.Name,
            server.Region.Code,
            server.PublicHost.Value,
            server.AgentBaseUrl.ToString(),
            server.Status.ToString(),
            server.Health.Reachable,
            server.Health.PeersActive,
            server.MaxPeers,
            server.Metrics.TrafficRxBytes,
            server.Metrics.TrafficTxBytes,
            server.Metrics.VpnSpeedMbps,
            server.Metrics.InfraLatencyMs
        );
    }
}