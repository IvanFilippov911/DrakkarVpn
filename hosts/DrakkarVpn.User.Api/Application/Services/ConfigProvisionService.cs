using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Peers.Application.Abstractions.Services;
using DrakkarVpn.Shared;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Services;

public sealed class ConfigProvisionService : IConfigProvisionService
{
    private readonly IPeerProvisionJobsService _jobs;
    private readonly IPeerProvisionPayloadFactory _payloadFactory;
    private readonly IServersQueryService _servers;

    public ConfigProvisionService(
        IPeerProvisionJobsService jobs,
        IPeerProvisionPayloadFactory payloadFactory,
        IServersQueryService servers)
    {
        _jobs = jobs;
        _payloadFactory = payloadFactory;
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

        var payload = _payloadFactory.CreateNew(server.PublicHost);

        var jobId = await _jobs.EnqueueAsync(
            new PeerProvisionJobCreateDto(
                UserId: access.UserId,
                DeviceId: access.DeviceId,
                ServerId: server.Id,
                MaxAttempts: 10,
                AgentPeerUuid: payload.AgentPeerUuid,
                ConfigRaw: payload.ConfigRaw),
            access.NowUtc,
            ct);

        return jobId;
    }
}

