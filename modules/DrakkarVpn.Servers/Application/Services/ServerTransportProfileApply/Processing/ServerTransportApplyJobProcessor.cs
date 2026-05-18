using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply;

public sealed class ServerTransportApplyJobProcessor : IServerTransportApplyJobProcessor
{
    private readonly IServerTransportApplyJobService _jobs;
    private readonly IAgentApplyServerTransportService _agentApply;
    private readonly IServerTransportApplyJobObsoleteGuard _obsoleteGuard;
    private readonly IServerTransportApplyJobOutcomeClassifier _outcomeClassifier;
    private readonly IServerTransportAppliedRecorder _appliedRecorder;
    private readonly ILogger<ServerTransportApplyJobProcessor> _log;

    public ServerTransportApplyJobProcessor(
        IServerTransportApplyJobService jobs,
        IAgentApplyServerTransportService agentApply,
        IServerTransportApplyJobObsoleteGuard obsoleteGuard,
        IServerTransportApplyJobOutcomeClassifier outcomeClassifier,
        IServerTransportAppliedRecorder appliedRecorder,
        ILogger<ServerTransportApplyJobProcessor> log)
    {
        _jobs = jobs;
        _agentApply = agentApply;
        _obsoleteGuard = obsoleteGuard;
        _outcomeClassifier = outcomeClassifier;
        _appliedRecorder = appliedRecorder;
        _log = log;
    }

    public async Task ProcessAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        string leaseOwner,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var actualJobs = await _obsoleteGuard.FilterActualOrMarkObsoleteAsync(jobs, leaseOwner, ct);
        if (actualJobs.Count == 0) return;

        var resultsByJobId = await _agentApply.ApplyAsync(actualJobs, ct);
        var buckets = _outcomeClassifier.Classify(actualJobs, resultsByJobId);

        if (buckets.Applied.Count > 0)
        {
            var affected = await _appliedRecorder.RecordAsync(buckets.Applied, leaseOwner, now, ct);
            LogBatchLeaseGuardMissIfNeeded(
                expected: buckets.Applied.Count,
                affected: affected,
                transition: "applied");
        }

        if (buckets.Retryable.Count > 0)
        {
            var affected = await _jobs.FailOrRescheduleAsync(
                buckets.Retryable,
                leaseOwner,
                now,
                Backoff,
                ct);
            LogBatchLeaseGuardMissIfNeeded(
                expected: buckets.Retryable.Count,
                affected: affected,
                transition: "retry/fail");
        }

        if (buckets.Permanent.Count > 0)
        {
            var affected = await _jobs.FailPermanentAsync(buckets.Permanent, leaseOwner, now, ct);
            LogBatchLeaseGuardMissIfNeeded(
                expected: buckets.Permanent.Count,
                affected: affected,
                transition: "permanent fail");
        }
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
