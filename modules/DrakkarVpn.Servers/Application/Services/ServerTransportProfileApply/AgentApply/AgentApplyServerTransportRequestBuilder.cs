using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Application.Options;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply.AgentApply;

public sealed class AgentApplyServerTransportRequestBuilder : IAgentApplyServerTransportRequestBuilder
{
    private readonly IAgentApplyServerTransportContextRepository _contexts;
    private readonly ServerTransportAgentApplyOptions _opt;

    public AgentApplyServerTransportRequestBuilder(
        IAgentApplyServerTransportContextRepository contexts,
        IOptions<ServerTransportAgentApplyOptions> options)
    {
        _contexts = contexts;
        _opt = options.Value;
    }

    public async Task<AgentApplyRequestBuildResult> BuildAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(jobs);
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
                || string.IsNullOrWhiteSpace(ctx.RealityFingerprint)
                || string.IsNullOrWhiteSpace(ctx.RealityPublicKey)
                || string.IsNullOrWhiteSpace(ctx.RealityDest))
            {
                failed[job.JobId] = new AgentApplyResult(
                    AgentApplyOutcome.PermanentFailed,
                    ErrorCode: "agent_apply_reality_settings_missing",
                    ErrorMessage:
                    $"Transport profile for activation '{job.ActivationId}' is missing required Reality settings (sni/shortId/fingerprint).");
                continue;
            }

            var request = new AgentApplyServerTransportRequest(
                OperationId: job.JobId,
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
                InboundTag: _opt.InboundTag,
                Flow: _opt.Flow,
                Encryption: _opt.Encryption);

            requests.Add(new PreparedAgentTransportApplyRequest(
                JobId: job.JobId,
                AgentBaseUrl: ctx.AgentBaseUrl,
                Request: request));
        }

        return new AgentApplyRequestBuildResult(requests, failed);
    }
}
