using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokeAllUserPeers;

public sealed record RevokeAllUserPeersCommand(Guid UserId) : IRequest<int>;