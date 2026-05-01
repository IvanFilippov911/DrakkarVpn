using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Domain.Enums.TransportProfile;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Servers.Application.Services;

public sealed class ServerTransportApplyJobProcessor : IServerTransportApplyJobProcessor
{
    private const string ErrorAgentApply = "ServerTransportAgentApplyFailed";

    private readonly IServerTransportApplyJobService _jobs;
    private readonly IAgentApplyServerTransportService _agentApply;
    private readonly ILogger<ServerTransportApplyJobProcessor> _log;

    public ServerTransportApplyJobProcessor(
        IServerTransportApplyJobService jobs,
        IAgentApplyServerTransportService agentApply,
        ILogger<ServerTransportApplyJobProcessor> log)
    {
        _jobs = jobs;
        _agentApply = agentApply;
        _log = log;
    }

    public async Task ProcessAsync(Guid jobId, CancellationToken ct)
    {
        var job = await LoadProcessableJobAsync(jobId, ct);
        if (job is null)
            return;

        try
        {
            var wire = await _agentApply.ApplyAsync(job.ServerId, job.ActivationId, ct);
            _log.LogInformation(
                "Agent apply completed for job {JobId} outcome={Outcome} payloadHash={PayloadHash} rollbackAttempted={RollbackAttempted} rollbackSucceeded={RollbackSucceeded}",
                job.JobId,
                wire.Outcome,
                wire.PayloadHash,
                wire.RollbackAttempted,
                wire.RollbackSucceeded);
            await CompleteAsync(job, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (ServerTransportAgentCallFailedException ex)
        {
            await HandleAgentFailureAsync(job, ex, ct);
        }
        catch (Exception ex)
        {
            await HandleUnexpectedFailureAsync(job, ex, ct);
        }
    }

    private async Task<ServerTransportApplyJobDto?> LoadProcessableJobAsync(
        Guid jobId,
        CancellationToken ct)
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("jobId is required", nameof(jobId));

        var job = await _jobs.GetAsync(jobId, ct);
        if (job is null)
        {
            _log.LogWarning("Server transport apply job {JobId} not found", jobId);
            return null;
        }

        if (job.State == ServerTransportApplyJobStatus.Completed)
            return null;

        if (job.State != ServerTransportApplyJobStatus.Processing)
        {
            _log.LogDebug(
                "Server transport apply job {JobId} skipped in state {State}",
                jobId,
                job.State);

            return null;
        }

        return job;
    }

    private async Task CompleteAsync(
        ServerTransportApplyJobDto job,
        CancellationToken ct)
    {
        var affected = await _jobs.MarkCompletedAsync(
            job,
            DateTime.UtcNow,
            ct);

        LogLeaseGuardMissIfNeeded(
            affected,
            job.JobId,
            "completion");
    }

    private async Task HandleAgentFailureAsync(
        ServerTransportApplyJobDto job,
        ServerTransportAgentCallFailedException ex,
        CancellationToken ct)
    {
        if (IsPermanentAgentFailure(ex.Code))
        {
            var affected = await _jobs.FailPermanentAsync(
                job,
                ex.Code,
                CompositeAgentErrorDetail(ex),
                DateTime.UtcNow,
                ct);

            LogLeaseGuardMissIfNeeded(
                affected,
                job.JobId,
                "permanent fail");

            return;
        }

        await FailOrRescheduleAsync(job, ex.Code, CompositeAgentErrorDetail(ex), ct);
    }

    private async Task HandleUnexpectedFailureAsync(
        ServerTransportApplyJobDto job,
        Exception ex,
        CancellationToken ct)
    {
        _log.LogError(
            ex,
            "Unexpected transport apply job failure {JobId}",
            job.JobId);

        await FailOrRescheduleAsync(
            job,
            ErrorAgentApply,
            ex.Message,
            ct);
    }

    private async Task FailOrRescheduleAsync(
        ServerTransportApplyJobDto job,
        string code,
        string message,
        CancellationToken ct)
    {
        var affected = await _jobs.FailOrRescheduleAsync(
            job,
            code,
            message,
            DateTime.UtcNow,
            Backoff,
            ct);

        LogLeaseGuardMissIfNeeded(
            affected,
            job.JobId,
            "retry/fail transition");
    }

    private void LogLeaseGuardMissIfNeeded(
        int affected,
        Guid jobId,
        string transition)
    {
        if (affected > 0)
            return;

        _log.LogDebug(
            "Server transport apply job {JobId} {Transition} skipped because lease/state guard did not match",
            jobId,
            transition);
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