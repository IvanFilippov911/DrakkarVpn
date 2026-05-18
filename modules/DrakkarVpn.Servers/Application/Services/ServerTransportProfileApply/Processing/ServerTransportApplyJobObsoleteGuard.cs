using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply;

public sealed class ServerTransportApplyJobObsoleteGuard : IServerTransportApplyJobObsoleteGuard
{
    private readonly IServerTransportApplyJobService _jobs;
    private readonly IServerTransportStateQueryService _transportStateQuery;
    private readonly ILogger<ServerTransportApplyJobObsoleteGuard> _log;

    public ServerTransportApplyJobObsoleteGuard(
        IServerTransportApplyJobService jobs,
        IServerTransportStateQueryService transportStateQuery,
        ILogger<ServerTransportApplyJobObsoleteGuard> log)
    {
        _jobs = jobs;
        _transportStateQuery = transportStateQuery;
        _log = log;
    }

    public async Task<IReadOnlyCollection<ServerTransportApplyJobDto>> FilterActualOrMarkObsoleteAsync(
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

        var desiredByServerId = await _transportStateQuery.GetTransportDesiredStatesAsync(serverIds, ct);

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
}
