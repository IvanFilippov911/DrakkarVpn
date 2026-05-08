using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Domain.Exceptions;
using DrakkarVpn.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Servers.Application.Services.ServerTransportProfileApply;

public sealed class ServerTransportAppliedRecorder : IServerTransportAppliedRecorder
{
    private readonly IServersUnitOfWork _uow;
    private readonly IServerRepository _servers;
    private readonly IServerTransportApplyJobRepository _jobs;
    private readonly ILogger<ServerTransportAppliedRecorder> _log;

    public ServerTransportAppliedRecorder(
        IServersUnitOfWork uow,
        IServerRepository servers,
        IServerTransportApplyJobRepository jobs,
        ILogger<ServerTransportAppliedRecorder> log)
    {
        _uow = uow;
        _servers = servers;
        _jobs = jobs;
        _log = log;
    }

    public Task<int> RecordAsync(
        IReadOnlyCollection<AppliedJobOutcome> applied,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (applied.Count == 0)
            return Task.FromResult(0);
        if (string.IsNullOrWhiteSpace(leaseOwner))
            throw new ArgumentException("leaseOwner is required", nameof(leaseOwner));

        return _uow.ExecuteInTransactionAsync(
            innerCt => RecordCoreAsync(applied, leaseOwner.Trim(), EnsureUtc(utcNow), innerCt),
            ct);
    }

    private async Task<int> RecordCoreAsync(
        IReadOnlyCollection<AppliedJobOutcome> applied,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct)
    {
        var serverIds = applied
            .Select(x => x.ServerId)
            .Distinct()
            .ToArray();

        var servers = await _servers.GetWithActivationsAsync(serverIds, ct);

        foreach (var outcome in applied)
        {
            if (!servers.TryGetValue(outcome.ServerId, out var server))
            {
                _log.LogWarning(
                    "Applied job references missing server. JobId={JobId} ServerId={ServerId}",
                    outcome.JobId,
                    outcome.ServerId);
                continue;
            }

            try
            {
                server.MarkTransportApplied(outcome.ActivationId, outcome.Version, utcNow);
            }
            catch (TransportAppliedVersionStaleException ex)
            {
                _log.LogInformation(
                    "Applied version is stale, skipping domain mutation. JobId={JobId} ServerId={ServerId} IncomingVersion={IncomingVersion} CurrentAppliedVersion={CurrentAppliedVersion}",
                    outcome.JobId,
                    outcome.ServerId,
                    ex.IncomingVersion,
                    ex.CurrentAppliedVersion);
            }
        }

        await _uow.SaveChangesAsync(ct);

        var jobIds = applied
            .Select(x => x.JobId)
            .ToArray();

        return await _jobs.MarkCompletedAsync(jobIds, leaseOwner, utcNow, ct);
    }

    private static DateTime EnsureUtc(DateTime dt)
        => DateTime.SpecifyKind(dt, DateTimeKind.Utc);
}
