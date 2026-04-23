using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Domain.Enums;
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

    public async Task ApplyAsync(Guid serverId, CancellationToken ct)
    {
        var server = await _servers.GetAsync(serverId, ct);
        if (server is null)
            throw new InvalidOperationException($"Server '{serverId}' was not found.");

        var activeActivation = server.TransportActivations
            .FirstOrDefault(x => x.Status == TransportActivationStatus.Active);

        if (activeActivation is null)
            throw new InvalidOperationException(
                $"Server '{serverId}' does not have an active transport activation.");

        var profile = await _profiles.GetAsync(activeActivation.TransportProfileId, ct);
        if (profile is null)
            throw new InvalidOperationException(
                $"Transport profile '{activeActivation.TransportProfileId}' was not found.");

        var request = new AgentApplyServerTransportRequest(
            ServerId: server.Id,
            ActivationId: activeActivation.Id,
            PublicHost: server.PublicHost.Value,
            PublicPort: server.PublicPort,
            TransportType: profile.TransportType.ToString(),
            SecurityType: profile.SecurityType.ToString(),
            RealitySni: profile.RealitySni!,
            RealityShortId: profile.RealityShortId!,
            RealityFingerprint: profile.RealityFingerprint!,
            RealityPublicKey: activeActivation.RealityPublicKey,
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

        await _agentClient.ApplyServerTransportAsync(agentBaseUrl, request, ct);

        _log.LogInformation(
            "Transport config applied on agent for server {ServerId}, activation {ActivationId}",
            server.Id,
            activeActivation.Id);
    }
}