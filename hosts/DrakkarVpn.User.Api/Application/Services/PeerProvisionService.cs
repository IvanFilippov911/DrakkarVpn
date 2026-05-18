using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;

namespace DrakkarVpn.Core.Api.Application.Services;

public sealed class PeerProvisionService : IPeerProvisionService
{
    private readonly IPeerProvisionJobsService _jobs;
    private readonly IServerConfigQueryService _serverConfig;
    private readonly IPeersQueryService _peersQuery;

    public PeerProvisionService(
        IPeerProvisionJobsService jobs,
        IServerConfigQueryService serverConfig,
        IPeersQueryService peersQuery)
    {
        _jobs = jobs;
        _serverConfig = serverConfig;
        _peersQuery = peersQuery;
    }

    public async Task<Guid> StartVpnConfigProvisioningAsync(
        VpnAccessContextDto access,
        CancellationToken ct)
    {
        var serverIds = await _serverConfig.GetEnabledServerIdsAsync(ct);
        if (serverIds.Length == 0)
            throw new InvalidOperationException("No enabled servers available");

        var onlineByServerId = await _peersQuery.GetServersOnlinePeersSummaryAsync(serverIds, ct);

        var serverId = serverIds
            .OrderBy(id => onlineByServerId.GetValueOrDefault(id, 0))
            .First();

        var agentPeerUuid = Guid.NewGuid();

        var jobId = await _jobs.EnqueueAsync(
            new PeerProvisionJobCreateDto(
                UserId: access.UserId,
                DeviceId: access.DeviceId,
                ServerId: serverId,
                MaxAttempts: 10,
                AgentPeerUuid: agentPeerUuid),
            access.NowUtc,
            ct);

        return jobId;
    }
}
