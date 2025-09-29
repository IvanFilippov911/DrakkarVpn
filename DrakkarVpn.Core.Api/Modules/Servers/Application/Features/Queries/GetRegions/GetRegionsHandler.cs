using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared.Servers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetRegions;

public sealed class GetRegionsHandler 
    : IRequestHandler<GetRegionsRequest, IReadOnlyList<RegionDto>>
{
    private readonly IServerRepository _repo;

    public GetRegionsHandler(IServerRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<RegionDto>> Handle(GetRegionsRequest request, CancellationToken ct)
    {
        var servers = await _repo.Query()
            .Where(s => s.Status == Domain.ServerStatus.Enabled)
            .ToListAsync(ct);

        if (servers is null || servers.Count == 0)
            return Array.Empty<RegionDto>();

        return servers
            .Select(s => new RegionDto(s.Region.Code))
            .Distinct()
            .OrderBy(r => r.Code)
            .ToList();

    }
}