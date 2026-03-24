using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Commads.RegisterPeer;

public sealed record RegisterPeerCommand(Guid PeerUuid) : IRequest;