using MediatR;
using NetworkMonitoring.Application.Abstractions.Services;
using NetworkMonitoring.Application.DTOs;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.NetworkMonitoring.GetProbeNodes;

public sealed class GetProbeNodesHandler
    : IRequestHandler<GetProbeNodesQuery, IReadOnlyList<ProbeNodeDto>>
{
    private readonly IProbeNodesQueryService _service;

    public GetProbeNodesHandler(IProbeNodesQueryService service)
        => _service = service;

    public Task<IReadOnlyList<ProbeNodeDto>> Handle(GetProbeNodesQuery q, CancellationToken ct)
        => _service.GetAllAsync(ct);
}

