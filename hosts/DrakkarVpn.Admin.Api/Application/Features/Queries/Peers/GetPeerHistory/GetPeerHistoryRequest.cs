using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetPeerHistory;

public sealed record GetPeerHistoryRequest(
    Guid PeerId,
    DateTime? FromUtc,
    DateTime? ToUtc
) : IRequest<IReadOnlyList<PeerMetricsHistoryDto>>;