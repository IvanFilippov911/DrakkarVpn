using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Shared.Errors;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokeAllUserPeers;

public sealed class RevokeAllUserPeersHandler
    : IRequestHandler<RevokeAllUserPeersCommand, int>
{
    private readonly IPeerRepository _peers;
    private readonly IServerRepository _servers;
    private readonly IPeerRevoker _revoker;

    public RevokeAllUserPeersHandler(
        IPeerRepository peers,
        IServerRepository servers,
        IPeerRevoker revoker)
    {
        _peers   = peers;
        _servers = servers;
        _revoker = revoker;
    }

    public async Task<int> Handle(RevokeAllUserPeersCommand cmd, CancellationToken ct)
    {
        var peers = await _peers.GetListActiveByUserAsync(cmd.UserId, ct);
        if (peers.Count == 0)
            return 0;
    
        var serverIds = peers.Select(p => p.ServerId).Distinct().ToArray();
        var servers   = await _servers.GetByIdsAsync(serverIds, ct);

        var revoked = 0;
        var failed  = 0;

        foreach (var peer in peers)
        {
            if (!servers.TryGetValue(peer.ServerId, out var server))
            {
                throw new InvalidOperationException(
                    $"Server {peer.ServerId} not loaded for peer {peer.Id} while revoking peers for user {cmd.UserId}");
            }

            var ok = await _revoker.RevokeAsync(peer, server, ct);
            if (ok) revoked++;
            else    failed++;
        }

        if (failed > 0)
            throw new PeersRevokeFailedException(cmd.UserId, revoked, failed);

        return revoked;
    }
}