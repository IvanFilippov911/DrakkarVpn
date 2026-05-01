using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Servers.Application.Services;

public sealed class AgentApplyServerTransportService : IAgentApplyServerTransportService
{
    private readonly IServerRepository _servers;
    private readonly ITransportProfileWriteRepository _profiles;
    private readonly IAgentTransportApiClient _agentClient;
    private readonly ILogger<AgentApplyServerTransportService> _log;

    public AgentApplyServerTransportService(
        IServerRepository servers,
        ITransportProfileWriteRepository profiles,
        IAgentTransportApiClient agentClient,
        ILogger<AgentApplyServerTransportService> log)
    {
        _servers = servers;
        _profiles = profiles;
        _agentClient = agentClient;
        _log = log;
    }

    public async Task<AgentApplyServerTransportWireResponse> ApplyAsync(Guid serverId, Guid activationId, CancellationToken ct)
    {
        var server = await _servers.GetAsync(serverId, ct);
        if (server is null)
            throw new InvalidOperationException($"Server '{serverId}' was not found.");

        var activation = server.TransportActivations
            .FirstOrDefault(x => x.Id == activationId);

        if (activation is null)
            throw new InvalidOperationException(
                $"Server '{serverId}' does not have an active transport activation.");

        var profile = await _profiles.GetAsync(activation.TransportProfileId, ct);
        if (profile is null)
            throw new InvalidOperationException(
                $"Transport profile '{activation.TransportProfileId}' was not found.");

        var request = new AgentApplyServerTransportRequest(
            OperationId: Guid.NewGuid(),
            ServerId: server.Id,
            ActivationId: activationId,
            PublicHost: server.PublicHost.Value,
            PublicPort: server.PublicPort,
            TransportType: profile.TransportType.ToString(),
            SecurityType: profile.SecurityType.ToString(),
            RealitySni: profile.RealitySni!,
            RealityShortId: profile.RealityShortId!,
            RealityFingerprint: profile.RealityFingerprint!,
            RealityPublicKey: activation.RealityPublicKey,
            RealityDest: profile.RealityDest,
            GrpcServiceName: profile.GrpcServiceName,
            GrpcAuthority: profile.GrpcAuthority,
            InboundTag: "vless-reality-in",
            Flow: "xtls-rprx-vision",
            Encryption: "none");

        var agentBaseUrl = server.AgentBaseUrl.ToString();
        if (string.IsNullOrWhiteSpace(agentBaseUrl))
        {
            throw new InvalidOperationException(
                $"Server '{serverId}' has no agent base URL configured.");
        }

        var wire = await _agentClient.ApplyServerTransportAsync(agentBaseUrl, request, ct);

        _log.LogInformation(
            "Transport config applied on agent for server {ServerId}, activation {ActivationId}: outcome={Outcome}, payloadHash={PayloadHash}, rollbackAttempted={RollbackAttempted}, rollbackSucceeded={RollbackSucceeded}",
            server.Id,
            activationId,
            wire.Outcome,
            wire.PayloadHash,
            wire.RollbackAttempted,
            wire.RollbackSucceeded);

        return wire;
    }
}