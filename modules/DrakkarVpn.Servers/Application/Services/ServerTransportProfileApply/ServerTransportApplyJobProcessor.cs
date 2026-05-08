using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply;

public sealed class ServerTransportApplyJobProcessor : IServerTransportApplyJobProcessor
{
    private const string ErrorMissingResult = "agent_apply_missing_result";
    private const string MissingResultMessage = "Agent apply service did not produce result for job.";

    private readonly IServerTransportApplyJobService _jobs;
    private readonly IAgentApplyServerTransportService _agentApply;
    private readonly IServersQueryService _serversQuery;
    private readonly IServerTransportAppliedRecorder _appliedRecorder;
    private readonly ILogger<ServerTransportApplyJobProcessor> _log;

    public ServerTransportApplyJobProcessor(
        IServerTransportApplyJobService jobs,
        IAgentApplyServerTransportService agentApply,
        IServersQueryService serversQuery,
        IServerTransportAppliedRecorder appliedRecorder,
        ILogger<ServerTransportApplyJobProcessor> log)
    {
        _jobs = jobs;
        _agentApply = agentApply;
        _serversQuery = serversQuery;
        _appliedRecorder = appliedRecorder;
        _log = log;
    }

    public async Task ProcessAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        string leaseOwner,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var actualJobs = await EnsureJobsActualOrMarkObsoleteAsync(jobs, leaseOwner, ct);
        if (actualJobs.Count == 0) return;

        var resultsByJobId = await _agentApply.ApplyAsync(actualJobs, ct);

        var applied = new List<AppliedJobOutcome>();
        var retryable = new List<ServerTransportApplyJobFailure>();
        var permanent = new List<ServerTransportApplyJobFailure>();

        foreach (var job in actualJobs)
        {
            if (!resultsByJobId.TryGetValue(job.JobId, out var result))
            {
                retryable.Add(new ServerTransportApplyJobFailure(
                    job,
                    ErrorMissingResult,
                    MissingResultMessage));
                continue;
            }

            switch (result.Outcome)
            {
                case AgentApplyOutcome.Applied:
                    applied.Add(new AppliedJobOutcome(
                        job.JobId,
                        job.ServerId,
                        job.ActivationId,
                        job.TargetTransportVersion));
                    break;

                case AgentApplyOutcome.PermanentFailed:
                    permanent.Add(new ServerTransportApplyJobFailure(
                        job,
                        result.ErrorCode ?? ErrorMissingResult,
                        result.ErrorMessage));
                    break;

                case AgentApplyOutcome.RetryableFailed:
                default:
                    retryable.Add(new ServerTransportApplyJobFailure(
                        job,
                        result.ErrorCode ?? ErrorMissingResult,
                        result.ErrorMessage));
                    break;
            }
        }

        if (applied.Count > 0)
        {
            var affected = await _appliedRecorder.RecordAsync(applied, leaseOwner, now, ct);
            LogBatchLeaseGuardMissIfNeeded(
                expected: applied.Count,
                affected: affected,
                transition: "applied");
        }

        if (retryable.Count > 0)
        {
            var affected = await _jobs.FailOrRescheduleAsync(retryable, leaseOwner, now, Backoff, ct);
            LogBatchLeaseGuardMissIfNeeded(
                expected: retryable.Count,
                affected: affected,
                transition: "retry/fail");
        }

        if (permanent.Count > 0)
        {
            var affected = await _jobs.FailPermanentAsync(permanent, leaseOwner, now, ct);
            LogBatchLeaseGuardMissIfNeeded(
                expected: permanent.Count,
                affected: affected,
                transition: "permanent fail");
        }
    }

    private async Task<IReadOnlyCollection<ServerTransportApplyJobDto>> EnsureJobsActualOrMarkObsoleteAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        string leaseOwner,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (jobs.Count == 0)
            return [];

        var serverIds = jobs
            .Select(x => x.ServerId)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();
        if (serverIds.Length == 0) return [];

        var desiredByServerId = await _serversQuery.GetTransportDesiredStatesAsync(serverIds, ct);

        var actual = new List<ServerTransportApplyJobDto>(jobs.Count);
        var obsoleteJobIds = new List<Guid>();

        foreach (var job in jobs)
        {
            var isObsolete =
                !desiredByServerId.TryGetValue(job.ServerId, out var desired)
                || job.TargetTransportVersion != desired.DesiredTransportVersion;

            if (isObsolete)
            {
                obsoleteJobIds.Add(job.JobId);
                continue;
            }

            actual.Add(job);
        }

        LogJobsClassified(
            total: jobs.Count,
            actual: actual.Count,
            obsolete: obsoleteJobIds.Count);

        if (obsoleteJobIds.Count == 0)
            return actual;

        var affected = await _jobs.MarkObsoleteAsync(
            obsoleteJobIds,
            leaseOwner,
            now,
            ct);

        LogBatchLeaseGuardMissIfNeeded(
            expected: obsoleteJobIds.Count,
            affected: affected,
            transition: "obsolete");

        return actual;
    }

    private void LogJobsClassified(
        int total,
        int actual,
        int obsolete)
    {
        _log.LogDebug(
            "Server transport apply jobs classified. Total={Total}, Actual={Actual}, Obsolete={Obsolete}",
            total,
            actual,
            obsolete);
    }

    private void LogBatchLeaseGuardMissIfNeeded(
        int expected,
        int affected,
        string transition)
    {
        if (affected == expected)
            return;

        _log.LogWarning(
            "Server transport apply jobs {Transition} transition affected fewer rows than expected. Expected={Expected}, Affected={Affected}, Skipped={Skipped}",
            transition,
            expected,
            affected,
            expected - affected);
    }

    private static TimeSpan Backoff(int attempt)
    {
        var baseSeconds = attempt switch
        {
            1 => 1,
            2 => 2,
            3 => 5,
            4 => 10,
            5 => 20,
            _ => 40
        };

        var jitterMs = Random.Shared.Next(0, 250);
        return TimeSpan.FromSeconds(baseSeconds) + TimeSpan.FromMilliseconds(jitterMs);
    }
}
