using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Queries.PeerMetrics;

public sealed record GetPeerMetricsQuery() : IRequest<IReadOnlyList<PeerMetricsDto>>;