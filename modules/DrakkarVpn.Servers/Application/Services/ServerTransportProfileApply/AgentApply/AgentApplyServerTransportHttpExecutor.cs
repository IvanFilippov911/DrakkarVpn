using System.Collections.Concurrent;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Servers.Application.Services;

public sealed class AgentApplyServerTransportHttpExecutor : IAgentApplyServerTransportHttpExecutor
{
    private const string ErrorAgentApply = "ServerTransportAgentApplyFailed";
    private const int MaxParallelRequests = 50;

    private readonly IAgentTransportApiClient _client;
    private readonly ILogger<AgentApplyServerTransportHttpExecutor> _log;

    public AgentApplyServerTransportHttpExecutor(
        IAgentTransportApiClient client,
        ILogger<AgentApplyServerTransportHttpExecutor> log)
    {
        _client = client;
        _log = log;
    }

    public async Task<IReadOnlyDictionary<Guid, AgentApplyResult>> ExecuteAsync(
        IReadOnlyList<PreparedAgentTransportApplyRequest> requests,
        CancellationToken ct)
    {
        if (requests.Count == 0)
            return new Dictionary<Guid, AgentApplyResult>();

        var results = new ConcurrentDictionary<Guid, AgentApplyResult>();

        var options = new ParallelOptions
        {
            CancellationToken = ct,
            MaxDegreeOfParallelism = Math.Min(requests.Count, MaxParallelRequests)
        };

        await Parallel.ForEachAsync(requests, options, async (req, token) =>
        {
            var result = await ExecuteOneAsync(req.AgentBaseUrl, req.Request, token);
            results[req.JobId] = result;
        });

        return results;
    }

    private async Task<AgentApplyResult> ExecuteOneAsync(
        string agentUrl,
        AgentApplyServerTransportRequest request,
        CancellationToken ct)
    {
        try
        {
            var result = await _client.ApplyServerTransportAsync(agentUrl, request, ct);
            return new AgentApplyResult(
                AgentApplyOutcome.Applied,
                PayloadHash: result.PayloadHash,
                Phase: result.Phase,
                RollbackAttempted: result.RollbackAttempted,
                RollbackSucceeded: result.RollbackSucceeded);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (ServerTransportAgentCallFailedException ex)
        {
            var outcome = IsPermanentAgentFailure(ex.Code)
                ? AgentApplyOutcome.PermanentFailed
                : AgentApplyOutcome.RetryableFailed;

            return new AgentApplyResult(
                outcome,
                ErrorCode: ex.Code,
                ErrorMessage: CompositeAgentErrorDetail(ex),
                Phase: ex.Phase,
                PayloadHash: ex.PayloadHash,
                RollbackAttempted: ex.RollbackAttempted,
                RollbackSucceeded: ex.RollbackSucceeded);
        }
        catch (Exception ex)
        {
            _log.LogError(
                ex,
                "Unexpected agent apply failure ServerId={ServerId} ActivationId={ActivationId}",
                request.ServerId,
                request.ActivationId);

            return new AgentApplyResult(
                AgentApplyOutcome.RetryableFailed,
                ErrorCode: ErrorAgentApply,
                ErrorMessage: ex.Message);
        }
    }

    private static string CompositeAgentErrorDetail(ServerTransportAgentCallFailedException ex)
    {
        var parts = new List<string>();
        parts.Add(ex.Message);
        if (!string.IsNullOrWhiteSpace(ex.Phase))
            parts.Add($"phase={ex.Phase}");
        if (!string.IsNullOrWhiteSpace(ex.PayloadHash))
            parts.Add($"payloadHash={ex.PayloadHash}");
        if (ex.RollbackAttempted is not null || ex.RollbackSucceeded is not null)
            parts.Add($"rollbackAttempted={ex.RollbackAttempted};rollbackSucceeded={ex.RollbackSucceeded}");
        return string.Join("; ", parts);
    }

    private static bool IsPermanentAgentFailure(string code)
    {
        if (!code.StartsWith("HTTP_", StringComparison.OrdinalIgnoreCase))
            return false;

        if (!int.TryParse(code["HTTP_".Length..], out var statusCode))
            return false;

        return statusCode is >= 400 and < 500 and not 408 and not 429;
    }
}