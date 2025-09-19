using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;

public sealed record RegisterPeerRequest(
    Guid UserId,
    Guid ServerId,
    DateTime? ExpiresAt
) : IRequest<PeerRegisterResponseDto>;