using System.Collections.Concurrent;
using DrakkarVpn.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply.AgentApply;

public sealed class AgentApplyServerTransportHttpExecutor : IAgentApplyServerTransportHttpExecutor
{
    private readonly IAgentTransportApiClient _client;
    private readonly ServerTransportAgentApplyOptions _opt;
    private readonly ILogger<AgentApplyServerTransportHttpExecutor> _log;

    public AgentApplyServerTransportHttpExecutor(
        IAgentTransportApiClient client,
        IOptions<ServerTransportAgentApplyOptions> options,
        ILogger<AgentApplyServerTransportHttpExecutor> log)
    {
        _client = client;
        _opt = options.Value;
        _log = log;
    }

    public async Task<IReadOnlyDictionary<Guid, AgentApplyResult>> ExecuteAsync(
        IReadOnlyList<PreparedAgentTransportApplyRequest> requests,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(requests);
        if (requests.Count == 0)
            return new Dictionary<Guid, AgentApplyResult>();

        var results = new ConcurrentDictionary<Guid, AgentApplyResult>();

        var options = new ParallelOptions
        {
            CancellationToken = ct,
            MaxDegreeOfParallelism = Math.Min(requests.Count, _opt.MaxParallelRequests)
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
            var call = await _client.ApplyServerTransportAsync(agentUrl, request, ct);
            return AgentTransportApplyOutcomeMapper.ToJobResult(call);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
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
                ErrorCode: "ServerTransportAgentApplyFailed",
                ErrorMessage: ex.Message);
        }
    }
}
