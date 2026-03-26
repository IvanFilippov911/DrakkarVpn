using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Errors;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;
using DrakkarVpn.Shared.InstanceProvider;
using DrakkarVpn.Users.Application.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Handlers.PeerCreate;

public sealed class PeerCreateHandler : IRequestHandler<PeerCreateCommand, Unit>
{
    private readonly IPeerProvisionJobsService     _jobs;
    private readonly IPeerAgentProvisioningService _agent;
    private readonly IPeersDomainCreateService     _domain;
    private readonly ILogger<PeerCreateHandler>    _log;
    private readonly IInstanceIdProvider           _instanceIdProvider;
    private readonly IDeviceLifecycleService _deviceLifecycleService;

    public PeerCreateHandler(
        IPeerProvisionJobsService jobs,
        IPeerAgentProvisioningService agent,
        IPeersDomainCreateService domain,
        ILogger<PeerCreateHandler> log,
        IInstanceIdProvider instanceIdProvider,
        IDeviceLifecycleService  deviceLifecycleService)
    {
        _jobs               = jobs;
        _agent              = agent;
        _domain             = domain;
        _log                = log;
        _instanceIdProvider = instanceIdProvider;
        _deviceLifecycleService = deviceLifecycleService;
    }

    public async Task<Unit> Handle(PeerCreateCommand cmd, CancellationToken ct)
    {
        var nowUtc     = DateTime.UtcNow;
        var instanceId = _instanceIdProvider.GetInstanceId();

        var batch = await _jobs.AcquireBatchAsync(
            take: cmd.BatchSize,
            lease: cmd.LeaseDuration,
            nowUtc: nowUtc,
            instanceId: instanceId,
            ct: ct);

        if (batch.Count == 0)
            return Unit.Value;
        
        var notDone = batch.Where(j => j.PeerId == null).ToList();

        var invalidPayload = notDone.Where(j => !IsValidPayload(j)).ToList();
        foreach (var j in invalidPayload)
        {
            await _jobs.FailPermanentAsync(
                jobSnapshot: j,
                errorCode: PeerProvisionErrorCodes.JobPayloadInvalid,
                errorMessage: "Job payload invalid: AgentPeerUuid/ConfigRaw required",
                nowUtc: nowUtc,
                ct: ct);
        }
        
        var invalidIds = invalidPayload.Select(x => x.JobId).ToHashSet();
        notDone = notDone.Where(x => !invalidIds.Contains(x.JobId)).ToList();
        if (notDone.Count == 0)
            return Unit.Value;

        var toCreateOnAgent = notDone
            .Where(j => j.AgentAppliedAtUtc == null)
            .ToList();
        
        var readyForCreateDomain = notDone
            .Where(j => j.AgentAppliedAtUtc != null)
            .ToList();

        var jobsById = notDone.ToDictionary(x => x.JobId);
        
        var domainDtos = new List<PeerBatchCreateDto>(notDone.Count);
        foreach (var j in readyForCreateDomain)
            domainDtos.Add(BuildDomainDtoFromJob(j));
        
        
        if (toCreateOnAgent.Count > 0)
        {
            await ProcessAgentProvisioningStepAsync(
                cmd,
                nowUtc,
                ct,
                jobsById,
                toCreateOnAgent,
                domainDtos);
        }
        
        await ProcessDomainCreationStepAsync(
            nowUtc,
            ct,
            jobsById,
            domainDtos);

        return Unit.Value;
    }

    private async Task ProcessAgentProvisioningStepAsync(
        PeerCreateCommand cmd,
        DateTime nowUtc,
        CancellationToken ct,
        IReadOnlyDictionary<Guid, PeerProvisionJob> jobsById,
        IReadOnlyList<PeerProvisionJob> toCreateOnAgent,
        List<PeerBatchCreateDto> domainDtos)
    {
        var agentResults = await _agent.ProvisionOnAgentsAsync(
            toCreateOnAgent,
            maxParallel: cmd.HttpConcurrency,
            nowUtc: nowUtc,
            ct: ct);
        
        var resultsByJobId = agentResults
            .GroupBy(x => x.JobId)
            .ToDictionary(g => g.Key, g => g.Last());
        
        foreach (var job in toCreateOnAgent)
        {
            if (resultsByJobId.ContainsKey(job.JobId))
                continue;
            
            var snap = jobsById[job.JobId];

            await _jobs.FailOrRescheduleAsync(
                jobSnapshot: snap,
                errorCode: PeerProvisionErrorCodes.AgentResultMissing,
                errorMessage: null,
                nowUtc: nowUtc,
                backoff: Backoff,
                ct: ct);
        }
        
        foreach (var r in resultsByJobId.Values)
        {
            if (!jobsById.TryGetValue(r.JobId, out var snap))
            {
                _log.LogWarning("Agent returned result for unknown JobId {JobId}", r.JobId);
                continue;
            }

            if (!TryNormalizeAgentResult(r, out var code, out var msg))
            {
                await _jobs.FailOrRescheduleAsync(snap, code, msg, nowUtc, Backoff, ct);
                continue;
            }
            
            try
            {
                await _jobs.MarkAgentAppliedAsync(
                    jobId: r.JobId,
                    nowUtc: nowUtc,
                    ct: ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                await _jobs.FailOrRescheduleAsync(
                    jobSnapshot: snap,
                    errorCode: PeerProvisionErrorCodes.JobCheckpointWriteFailed,
                    errorMessage: ex.Message,
                    nowUtc: nowUtc,
                    backoff: Backoff,
                    ct: ct);
                continue;
            }

            domainDtos.Add(new PeerBatchCreateDto(
                JobId: snap.JobId,
                UserId: snap.UserId,
                ServerId: snap.ServerId,
                AgentPeerUuid: snap.AgentPeerUuid!.Value,
                DeviceId: snap.DeviceId));
        }
    }

    private async Task ProcessDomainCreationStepAsync(
        DateTime nowUtc,
        CancellationToken ct,
        IReadOnlyDictionary<Guid, PeerProvisionJob> jobsById,
        List<PeerBatchCreateDto> domainDtos)
    {
        if (domainDtos.Count == 0)
            return;
        
        domainDtos = domainDtos
            .GroupBy(x => x.DeviceId, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.Last())
            .ToList();

        var creationResults = await _domain.CreateBatchAsync(domainDtos, nowUtc, ct);

        foreach (var res in creationResults)
        {
            if (!jobsById.TryGetValue(res.JobId, out var snap))
            {
                _log.LogWarning("Domain returned result for unknown JobId {JobId}", res.JobId);
                continue;
            }

            if (res.Success)
            {
                await _jobs.MarkReadyAsync(
                    jobId: res.JobId,
                    peerId: res.PeerId!.Value,
                    nowUtc: nowUtc,
                    ct: ct);
                
                await _deviceLifecycleService.ActivateAsync(snap.DeviceId, ct);
                continue;
            }

            await _jobs.FailOrRescheduleAsync(
                jobSnapshot: snap,
                errorCode: res.ErrorCode ?? PeerProvisionErrorCodes.DomainPeerCreationFailed,
                errorMessage: res.ErrorMessage,
                nowUtc: nowUtc,
                backoff: Backoff,
                ct: ct);
        }
    }

    private static PeerBatchCreateDto BuildDomainDtoFromJob(PeerProvisionJob j)
        => new(
            JobId: j.JobId,
            UserId: j.UserId,
            ServerId: j.ServerId,
            AgentPeerUuid: j.AgentPeerUuid!.Value,
            DeviceId: j.DeviceId);

    private static bool TryNormalizeAgentResult(
        AgentProvisionAttemptResult r,
        out string errorCode,
        out string? errorMessage)
    {
        errorCode = string.Empty;
        errorMessage = null;

        if (!r.Applied)
        {
            errorCode = r.ErrorCode ?? PeerProvisionErrorCodes.AgentApplyFailed;
            errorMessage = r.ErrorMessage;
            return false;
        }

        return true;
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
    
    private static bool IsValidPayload(PeerProvisionJob j)
        => j.AgentPeerUuid.HasValue
           && j.AgentPeerUuid.Value != Guid.Empty;
}