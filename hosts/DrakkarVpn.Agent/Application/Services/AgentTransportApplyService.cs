using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Application.DTOs.Enums;

namespace DrakkarVpn.Agent.Application.Services;

//Этот сервис еще в доработке
public sealed class AgentTransportApplyService : IAgentTransportApplyService
{
    private static readonly SemaphoreSlim ApplyLock = new(1, 1);

    private readonly IAgentTransportPayloadHashService _hash;
    private readonly IAgentTransportStateService _state;
    private readonly IXrayTransportConfigApplyService _xrayApply;
    private readonly ILogger<AgentTransportApplyService> _log;

    public AgentTransportApplyService(
        IAgentTransportPayloadHashService hash,
        IAgentTransportStateService state,
        IXrayTransportConfigApplyService xrayApply,
        ILogger<AgentTransportApplyService> log)
    {
        _hash = hash;
        _state = state;
        _xrayApply = xrayApply;
        _log = log;
    }

    public async Task<AgentTransportApplyResult> ApplyAsync(ApplyServerTransportRequestDto request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        await ApplyLock.WaitAsync(ct);
        try
        {
            var hashStep = CalculateHash(request, ct);
            if (hashStep.ErrorIfAny is not null)
                return hashStep.ErrorIfAny;

            var payloadHash = hashStep.PayloadHash!;

            var stateStep = await GetCurrentStateOrErrorAsync(request, payloadHash, ct);
            if (stateStep.ErrorIfAny is not null)
                return stateStep.ErrorIfAny;

            if (IsAlreadyApplied(stateStep.State, payloadHash))
                return OkAlreadyApplied(request, payloadHash);

            var xrayError = await _xrayApply.ApplyAsync(request, payloadHash, ct);
            if (xrayError is not null)
                return xrayError;

            var persistError = await PersistStateOrErrorAsync(request, payloadHash, ct);
            if (persistError is not null)
                return persistError;

            return OkApplied(request, payloadHash);
        }
        finally
        {
            ApplyLock.Release();
        }
    }

    private HashStep CalculateHash(ApplyServerTransportRequestDto request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        try
        {
            var hash = _hash.Calculate(request);
            return new HashStep(null, hash);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (System.Exception ex)
        {
            _log.LogError(ex, "Payload hash computation failed OperationId={OperationId} ServerId={ServerId}",
                request.OperationId, request.ServerId);
            var err = AgentTransportApplyResult.Failed(
                AgentTransportApplyPhase.Hashing,
                "hash_failed",
                PublicMessages.HashFailed,
                null);
            return new HashStep(err, null);
        }
    }

    private async Task<StateStep> GetCurrentStateOrErrorAsync(
        ApplyServerTransportRequestDto request,
        string payloadHash,
        CancellationToken ct)
    {
        try
        {
            var current = await _state.GetCurrentAsync(ct);
            return new StateStep(null, current);
        }
        catch (System.Exception ex)
        {
            _log.LogError(
                ex,
                "Transport apply state read failed OperationId={OperationId} ServerId={ServerId} PayloadHash={PayloadHash}",
                request.OperationId,
                request.ServerId,
                payloadHash);
            var err = AgentTransportApplyResult.Failed(
                AgentTransportApplyPhase.StateRead,
                "state_read_failed",
                PublicMessages.StateReadFailed,
                payloadHash);
            return new StateStep(err, null);
        }
    }

    private static bool IsAlreadyApplied(AgentTransportStateDto? current, string payloadHash)
    {
        return current is not null
               && string.Equals(current.PayloadHash, payloadHash, StringComparison.Ordinal);
    }

    private AgentTransportApplyResult OkAlreadyApplied(ApplyServerTransportRequestDto request, string payloadHash)
    {
        _log.LogInformation(
            "Transport config already applied OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
        return AgentTransportApplyResult.AlreadyApplied(payloadHash);
    }

    private async Task<AgentTransportApplyResult?> PersistStateOrErrorAsync(
        ApplyServerTransportRequestDto request,
        string payloadHash,
        CancellationToken ct)
    {
        try
        {
            var now = DateTime.UtcNow;
            await _state.MarkAppliedAsync(
                request.ServerId,
                request.ActivationId,
                request.OperationId,
                payloadHash,
                now,
                ct);
            return null;
        }
        catch (System.Exception ex)
        {
            _log.LogError(
                ex,
                "Transport state persist failed OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
                request.OperationId,
                request.ServerId,
                request.ActivationId,
                payloadHash);
            return AgentTransportApplyResult.Failed(
                AgentTransportApplyPhase.StatePersist,
                "state_persist_failed",
                PublicMessages.StatePersistFailed,
                payloadHash);
        }
    }

    private AgentTransportApplyResult OkApplied(ApplyServerTransportRequestDto request, string payloadHash)
    {
        _log.LogInformation(
            "Transport config applied OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
        return AgentTransportApplyResult.Applied(payloadHash);
    }

    private readonly record struct HashStep(AgentTransportApplyResult? ErrorIfAny, string? PayloadHash);

    private readonly record struct StateStep(AgentTransportApplyResult? ErrorIfAny, AgentTransportStateDto? State);

    private static class PublicMessages
    {
        public const string HashFailed = "Unable to compute transport fingerprint.";
        public const string StateReadFailed = "Unable to load applied transport state.";
        public const string StatePersistFailed = "Unable to persist applied transport state.";
    }
}
