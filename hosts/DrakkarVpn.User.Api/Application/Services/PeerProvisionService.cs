using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Application.Services;

public sealed class PeerProvisionService : IPeerProvisionService
{
    private readonly IPeerProvisionJobsService _jobs;
    private readonly IServersQueryService _servers;

    public PeerProvisionService(
        IPeerProvisionJobsService jobs,
        IServersQueryService servers)
    {
        _jobs = jobs;
        _servers = servers;
    }

    public async Task<Guid> StartVpnConfigProvisioningAsync(
        VpnAccessContextDto access,
        CancellationToken ct)
    {
        var servers = await _servers.GetListAsync(nameof(ServerStatus.Enabled), ct);
        var server = servers
                         .OrderBy(s => s.PeersActive)
                         .FirstOrDefault()
                     ?? throw new InvalidOperationException("No enabled servers available");

        var agentPeerUuid = Guid.NewGuid();

        var jobId = await _jobs.EnqueueAsync(
            new PeerProvisionJobCreateDto(
                UserId: access.UserId,
                DeviceId: access.DeviceId,
                ServerId: server.Id,
                MaxAttempts: 10,
                AgentPeerUuid: agentPeerUuid),
            access.NowUtc,
            ct);

        return jobId;
    }
}

