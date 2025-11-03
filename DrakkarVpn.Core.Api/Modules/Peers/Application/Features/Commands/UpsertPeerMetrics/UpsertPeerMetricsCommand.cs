using DrakkarVpn.Shared.Peers;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.UpsertPeerMetrics;

public sealed record UpsertPeerMetricsCommand(
    Guid ServerId,
    IReadOnlyList<PeerMetricsDto> Items
) : IRequest<Unit>;