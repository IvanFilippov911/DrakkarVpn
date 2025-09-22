using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.DeletePeer;

public sealed record DeletePeerRequest(Guid PeerId) : IRequest<bool>;