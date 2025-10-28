using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.VO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed class GetServersHandler : IRequestHandler<GetServersRequest, IReadOnlyList<GetServerDto>>
{
    private readonly IServerRepository _repo;
    public GetServersHandler(IServerRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<GetServerDto>> Handle(GetServersRequest request, CancellationToken ct)
    {
        var query = _repo.Query();

        if (!string.IsNullOrWhiteSpace(request.Region))
            query = query.Where(s => s.Region == new Region(request.Region));

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<ServerStatus>(request.Status, true, out var status))
            query = query.Where(s => s.Status == status);

        return await query
            .OrderByDescending(s => s.Health.Reachable)
            .ThenBy(s => s.Health.PeersActive)
            .Select(s => new GetServerDto(
                s.Id,
                s.Name,
                s.Region.Code,
                s.Status.ToString(),
                s.Health.Reachable,
                s.Health.PeersActive,
                s.MaxPeers,
                s.Metrics.VpnSpeedMbps,
                s.Metrics.InfraLatencyMs
            ))
            .ToListAsync(ct);
    }
}