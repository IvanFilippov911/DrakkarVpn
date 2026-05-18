using DrakkarVpn.Core.Api.Application.Features.Queries.GetPeerByUuid;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerByUuid;

public sealed class GetConfigVpnByUuidHandler
    : IRequestHandler<GetConfigVpnByUuidQuery, string?>
{
    private readonly IPeersQueryService _peers;
    private readonly IServerConfigQueryService _servers;
    private readonly IVpnBuildArtifactsService _builder;

    public GetConfigVpnByUuidHandler(
        IPeersQueryService peers,
        IServerConfigQueryService servers,
        IVpnBuildArtifactsService builder)
    {
        _peers = peers;
        _servers = servers;
        _builder = builder;
    }

    public async Task<string?> Handle(GetConfigVpnByUuidQuery q, CancellationToken ct)
    {
        var peer = await _peers.GetDataForConfigByAgentUuidAsync(q.PeerUuid, ct);
        if (peer is null)
            return null;

        var server = await _servers.GetDataForConfigByIdAsync(peer.ServerId, ct);
        if (server is null)
            throw new InvalidOperationException($"Server '{peer.ServerId}' not found.");

        return _builder.BuildXrayClientConfig(peer, server);
    }
}