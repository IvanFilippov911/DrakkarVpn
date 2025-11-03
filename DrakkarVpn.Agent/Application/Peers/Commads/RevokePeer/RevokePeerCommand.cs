using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Commads.RevokePeer;

public sealed record RevokePeerCommand(Guid PeerUuid) : IRequest<bool>;