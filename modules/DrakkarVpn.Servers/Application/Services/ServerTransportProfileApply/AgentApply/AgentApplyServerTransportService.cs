using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply.AgentApply;

public sealed class AgentApplyServerTransportService : IAgentApplyServerTransportService
{
    private readonly IAgentApplyServerTransportRequestBuilder _builder;
    private readonly IAgentApplyServerTransportHttpExecutor _httpExecutor;
    private readonly ILogger<AgentApplyServerTransportService> _log;

    public AgentApplyServerTransportService(
        IAgentApplyServerTransportRequestBuilder builder,
        IAgentApplyServerTransportHttpExecutor httpExecutor,
        ILogger<AgentApplyServerTransportService> log)
    {
        _builder = builder;
        _httpExecutor = httpExecutor;
        _log = log;
    }
    
    public async Task<IReadOnlyDictionary<Guid, AgentApplyResult>> ApplyAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs, CancellationToken ct)
    {
        if (jobs.Count == 0)
            return new Dictionary<Guid, AgentApplyResult>();
        
        var results = new Dictionary<Guid, AgentApplyResult>(jobs.Count);
        
        var buildResult = await _builder.BuildAsync(jobs, ct);

        foreach (var failed in buildResult.Failed)
        {
            results[failed.Key] = failed.Value;
        }
        
        if (buildResult.Requests.Count == 0)
            return results;

        var httpResults = await _httpExecutor.ExecuteAsync(
            buildResult.Requests, ct);
        
        foreach (var result in httpResults)
        {
            results[result.Key] = result.Value;
        }
        
        AddMissingResultsAsRetryable(
            jobs,
            results);

        LogApplyBatchResult(jobs.Count, results);

        return results.AsReadOnly();;
    }
    
    private static void AddMissingResultsAsRetryable(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        Dictionary<Guid, AgentApplyResult> results)
    {
        foreach (var job in jobs)
        {
            if (results.ContainsKey(job.JobId))
                continue;

            results[job.JobId] = new AgentApplyResult(
                AgentApplyOutcome.RetryableFailed,
                ErrorCode: "agent_apply_missing_result",
                ErrorMessage: "Agent apply service did not produce result for job.");
        }
    }

    private void LogApplyBatchResult(
        int total,
        IReadOnlyDictionary<Guid, AgentApplyResult> results)
    {
        var applied = results.Count(x => x.Value.Outcome == AgentApplyOutcome.Applied);
        var retryable = results.Count(x => x.Value.Outcome == AgentApplyOutcome.RetryableFailed);
        var permanent = results.Count(x => x.Value.Outcome == AgentApplyOutcome.PermanentFailed);

        _log.LogDebug(
            "Server transport agent apply batch completed. Total={Total}, Applied={Applied}, RetryableFailed={RetryableFailed}, PermanentFailed={PermanentFailed}",
            total,
            applied,
            retryable,
            permanent);
    }
}