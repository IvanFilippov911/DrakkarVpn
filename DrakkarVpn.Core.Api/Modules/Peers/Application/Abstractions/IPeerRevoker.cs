using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerRevoker
{
    Task<bool> RevokeAsync(Peer peer, Server server, CancellationToken ct);
}