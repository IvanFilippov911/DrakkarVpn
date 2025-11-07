using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerHistory;

public sealed record GetPeerHistoryRequest(
    Guid PeerId,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null
) : IRequest<IReadOnlyList<PeerMetricsHistoryDto>>;