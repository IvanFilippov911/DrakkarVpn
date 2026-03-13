using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Peers.RevokePeer;

public sealed record RevokePeerRequest(
    Guid PeerId,
    Guid ServerId
) : IRequest<bool>, IPeersCommand<bool>;