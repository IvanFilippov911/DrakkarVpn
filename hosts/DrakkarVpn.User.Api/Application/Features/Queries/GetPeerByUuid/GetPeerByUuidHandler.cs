using DrakkarVpn.Core.Api.Application.Features.Queries.GetPeerByUuid;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerByUuid;

public sealed class GetPeerByUuidHandler
    : IRequestHandler<GetPeerByUuidQuery, string?>
{
    private readonly IPeersQueryService _peers;
    private readonly IServersQueryService _servers;
    private readonly IVpnBuildArtifactsService _builder;

    public GetPeerByUuidHandler(
        IPeersQueryService peers,
        IServersQueryService servers,
        IVpnBuildArtifactsService builder)
    {
        _peers = peers;
        _servers = servers;
        _builder = builder;
    }

    public async Task<string?> Handle(GetPeerByUuidQuery q, CancellationToken ct)
    {
        var peer = await _peers.GetDataForConfigByAgentUuidAsync(q.PeerUuid, ct);
        if (peer is null)
            return null;

        var server = await _servers.GetDataForConfigByIdAsync(peer.ServerId, ct);
        if (server is null)
            throw new InvalidOperationException($"Server '{peer.ServerId}' not found.");

        return _builder.BuildConfig(peer, server);
    }
}