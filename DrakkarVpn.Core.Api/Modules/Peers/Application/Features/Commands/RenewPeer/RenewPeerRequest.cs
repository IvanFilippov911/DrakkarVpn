using MediatR;
namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RenewPeer;

public sealed record RenewPeerRequest(Guid PeerId, DateTime NewExpiresAt) : IRequest<bool>;