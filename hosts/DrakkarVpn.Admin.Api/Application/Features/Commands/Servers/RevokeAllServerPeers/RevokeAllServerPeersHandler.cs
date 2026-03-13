using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RevokeAllServerPeers;

public sealed class RevokeAllServerPeersHandler
    : IRequestHandler<RevokeAllServerPeersCommand, int>
{
    private readonly IPeerRevocationService _revocation;

    public RevokeAllServerPeersHandler(IPeerRevocationService revocation)
        => _revocation = revocation;

    public Task<int> Handle(
        RevokeAllServerPeersCommand c,
        CancellationToken ct)
        => _revocation.RevokeAllServerPeersAsync(c.ServerId, ct);
}