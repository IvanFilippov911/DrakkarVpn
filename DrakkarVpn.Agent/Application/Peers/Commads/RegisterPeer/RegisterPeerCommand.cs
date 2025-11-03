using DrakkarVpn.Agent.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Commads.RegisterPeer;

public sealed record RegisterPeerCommand() 
    : IRequest<RegisterPeerResponseDto>;