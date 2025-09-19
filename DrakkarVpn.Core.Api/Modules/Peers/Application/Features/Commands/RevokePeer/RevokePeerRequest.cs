using MediatR;
namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokePeer;

public sealed record RevokePeerRequest(Guid PeerId, Guid ServerId) : IRequest<bool>;