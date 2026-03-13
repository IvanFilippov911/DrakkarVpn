using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Peers.RevokePeer;

public sealed class RevokePeerHandler : IRequestHandler<RevokePeerRequest, bool>
{
    private readonly IPeerRevocationService _revocation;

    public RevokePeerHandler(IPeerRevocationService revocation) => _revocation = revocation;

    public Task<bool> Handle(RevokePeerRequest c, CancellationToken ct)
        => _revocation.RevokePeerAsync(c.ServerId, c.PeerId, ct);
}