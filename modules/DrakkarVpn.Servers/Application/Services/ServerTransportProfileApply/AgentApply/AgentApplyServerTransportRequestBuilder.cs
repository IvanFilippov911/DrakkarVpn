using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Servers.Application.Services;

public sealed class AgentApplyServerTransportRequestBuilder : IAgentApplyServerTransportRequestBuilder
{
    private const string InboundTag = "vless-reality-in";
    private const string Flow = "xtls-rprx-vision";
    private const string Encryption = "none";

    private readonly IAgentApplyServerTransportContextRepository _contexts;

    public AgentApplyServerTransportRequestBuilder(IAgentApplyServerTransportContextRepository contexts)
    {
        _contexts = contexts;
    }

    public async Task<AgentApplyRequestBuildResult> BuildAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        CancellationToken ct)
    {
        if (jobs.Count == 0)
        {
            return new AgentApplyRequestBuildResult(
                Array.Empty<PreparedAgentTransportApplyRequest>(),
                new Dictionary<Guid, AgentApplyResult>());
        }

        var activationIds = jobs
            .Select(x => x.ActivationId)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        var contexts = activationIds.Length == 0
            ? new Dictionary<Guid, AgentApplyServerTransportContext>()
            : await _contexts.GetByActivationIdsAsync(activationIds, ct);

        var requests = new List<PreparedAgentTransportApplyRequest>(jobs.Count);
        var failed = new Dictionary<Guid, AgentApplyResult>();

        foreach (var job in jobs)
        {
            if (!contexts.TryGetValue(job.ActivationId, out var ctx))
            {
                failed[job.JobId] = new AgentApplyResult(
                    AgentApplyOutcome.PermanentFailed,
                    ErrorCode: "agent_apply_context_not_found",
                    ErrorMessage:
                    $"Server '{job.ServerId}' or activation '{job.ActivationId}' or its transport profile was not found.");
                continue;
            }

            if (ctx.ServerId != job.ServerId)
            {
                failed[job.JobId] = new AgentApplyResult(
                    AgentApplyOutcome.PermanentFailed,
                    ErrorCode: "agent_apply_server_mismatch",
                    ErrorMessage:
                    $"Activation '{job.ActivationId}' belongs to server '{ctx.ServerId}', but job targets server '{job.ServerId}'.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(ctx.RealitySni)
                || string.IsNullOrWhiteSpace(ctx.RealityShortId)
                || string.IsNullOrWhiteSpace(ctx.RealityFingerprint))
            {
                failed[job.JobId] = new AgentApplyResult(
                    AgentApplyOutcome.PermanentFailed,
                    ErrorCode: "agent_apply_reality_settings_missing",
                    ErrorMessage:
                    $"Transport profile for activation '{job.ActivationId}' is missing required Reality settings (sni/shortId/fingerprint).");
                continue;
            }

            var request = new AgentApplyServerTransportRequest(
                OperationId: Guid.NewGuid(),
                ServerId: ctx.ServerId,
                ActivationId: ctx.ActivationId,
                PublicHost: ctx.PublicHost,
                PublicPort: ctx.PublicPort,
                TransportType: ctx.TransportType.ToString(),
                SecurityType: ctx.SecurityType.ToString(),
                RealitySni: ctx.RealitySni!,
                RealityShortId: ctx.RealityShortId!,
                RealityFingerprint: ctx.RealityFingerprint!,
                RealityPublicKey: ctx.RealityPublicKey,
                RealityDest: ctx.RealityDest,
                GrpcServiceName: ctx.GrpcServiceName,
                GrpcAuthority: ctx.GrpcAuthority,
                InboundTag: InboundTag,
                Flow: Flow,
                Encryption: Encryption);

            requests.Add(new PreparedAgentTransportApplyRequest(
                JobId: job.JobId,
                AgentBaseUrl: ctx.AgentBaseUrl,
                Request: request));
        }

        return new AgentApplyRequestBuildResult(requests, failed);
    }
}
