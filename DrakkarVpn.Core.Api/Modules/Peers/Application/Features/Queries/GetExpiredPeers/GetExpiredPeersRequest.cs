using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetExpiredPeers;

public sealed record GetExpiredPeersRequest(DateTime Until) 
    : IRequest<IReadOnlyList<PeerResponseDto>>;