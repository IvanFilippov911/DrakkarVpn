using System.Collections.Concurrent;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Errors;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using DrakkarVpn.Shared.Servers;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Services;

public sealed class PeerAgentProvisioningService : IPeerAgentProvisioningService
{
    private readonly IServerQueryForPeers _serversQuery;
    private readonly IPeersAgentClient _agent;
    private readonly ILogger<PeerAgentProvisioningService> _log;

    public PeerAgentProvisioningService(
        IServerQueryForPeers serversQuery,
        IPeersAgentClient agent,
        ILogger<PeerAgentProvisioningService> log)
    {
        _serversQuery = serversQuery;
        _agent        = agent;
        _log          = log;
    }

    public async Task<IReadOnlyList<AgentProvisionAttemptResult>> ProvisionOnAgentsAsync(
        IReadOnlyList<PeerProvisionJob> jobs,
        int maxParallel,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (jobs is null) throw new ArgumentNullException(nameof(jobs));
        if (jobs.Count == 0) return Array.Empty<AgentProvisionAttemptResult>();

        nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);
        if (maxParallel <= 0) maxParallel = 1;

        var serverIds = jobs.Select(j => j.ServerId).Distinct().ToArray();
        
        var baseUrls = await _serversQuery.GetAgentBaseUrlsByIdsAsync(serverIds, ct);

        using var sem = new SemaphoreSlim(maxParallel);
        var results = new ConcurrentBag<AgentProvisionAttemptResult>();

        var tasks = jobs.Select(async job =>
        {
            await sem.WaitAsync(ct);
            try
            {
                if (!baseUrls.TryGetValue(job.ServerId, out var baseUrl))
                {
                    results.Add(AgentProvisionAttemptResultFactory.Fail(
                        job.JobId,
                        PeerProvisionErrorCodes.ServerNotLoaded,
                        "AgentBaseUrl not resolved",
                        nowUtc));
                    return;
                }
                
                await _agent.RegisterPeerAsync(baseUrl, job.AgentPeerUuid!.Value, ct);

                results.Add(AgentProvisionAttemptResultFactory.Applied(job.JobId, nowUtc));
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (PeersAgentAlreadyExistsException)
            {
                results.Add(AgentProvisionAttemptResultFactory.Applied(job.JobId, nowUtc));
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex, "Agent apply failed for job {JobId}", job.JobId);

                results.Add(AgentProvisionAttemptResultFactory.Fail(
                    job.JobId,
                    PeerProvisionErrorCodes.AgentApplyFailed,
                    ex.Message,
                    nowUtc));
            }
            finally
            {
                sem.Release();
            }
        });

        await Task.WhenAll(tasks);
        return results.ToList();
    }
}