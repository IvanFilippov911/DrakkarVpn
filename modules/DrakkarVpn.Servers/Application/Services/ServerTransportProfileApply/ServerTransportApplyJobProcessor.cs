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
        if (jobId == Guid.Empty) throw new ArgumentException("jobId is required", nameof(jobId));
        
        var job = await _jobs.GetAsync(jobId, ct);
        if (job is null)
        {
            _log.LogWarning("Server transport apply job {JobId} not found", jobId);
            return;
        }

        if (job.State == ServerTransportApplyJobStatus.Completed)
            return;

        if (job.State != ServerTransportApplyJobStatus.Processing)
        {
            _log.LogDebug("Server transport apply job {JobId} skipped in state {State}", jobId, job.State);
            return;
        }

        try
        {
            await _agentApply.ApplyAsync(job.ServerId, job.ActivationId, ct);
            await _jobs.MarkCompletedAsync(job.JobId, DateTime.UtcNow, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (ServerTransportAgentCallFailedException ex)
        {
            if (IsPermanentAgentFailure(ex.Code))
            {
                await _jobs.FailPermanentAsync(job, ex.Code, ex.Message, DateTime.UtcNow, ct);
                return;
            }

            await FailOrRescheduleAsync(job, ex.Code, ex.Message, ct);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected transport apply job failure {JobId}", job.JobId);

            await FailOrRescheduleAsync(job, ErrorAgentApply, ex.Message, ct);
        }
    }

    private Task FailOrRescheduleAsync(
        ServerTransportApplyJobDto job,
        string code,
        string message,
        CancellationToken ct)
    {
        return _jobs.FailOrRescheduleAsync(
            job,
            code,
            message,
            DateTime.UtcNow,
            Backoff,
            ct);
    }

    private static bool IsPermanentAgentFailure(string code)
    {
        if (!code.StartsWith("HTTP_", StringComparison.OrdinalIgnoreCase))
            return false;

        if (!int.TryParse(code["HTTP_".Length..], out var statusCode))
            return false;

        return statusCode is >= 400 and < 500 and not 408;
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