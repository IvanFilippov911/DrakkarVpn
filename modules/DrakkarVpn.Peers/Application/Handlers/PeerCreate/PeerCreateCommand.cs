using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Handlers.PeerCreate;

public record PeerCreateCommand(
    int BatchSize = 50,
    TimeSpan LeaseDuration = default,
    TimeSpan StuckTimeout = default,
    int HttpConcurrency = 10
) : IRequest<Unit>, IPeersCommand<Unit>;