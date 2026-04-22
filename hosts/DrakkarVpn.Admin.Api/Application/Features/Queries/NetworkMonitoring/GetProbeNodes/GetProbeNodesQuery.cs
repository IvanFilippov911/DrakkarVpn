using MediatR;
using NetworkMonitoring.Application.DTOs;

namespace DrakkarVpn.Admin.Api.Application.Features.Queries.NetworkMonitoring.GetProbeNodes;

public sealed record GetProbeNodesQuery()
    : IRequest<IReadOnlyList<ProbeNodeDto>>;

