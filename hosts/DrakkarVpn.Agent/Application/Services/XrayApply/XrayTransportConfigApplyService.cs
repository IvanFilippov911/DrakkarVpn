using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Application.DTOs.Enums;
using DrakkarVpn.Agent.Application.Exception;

namespace DrakkarVpn.Agent.Application.Services.XrayApply;


//Этот сервис еще в доработке
public sealed class XrayTransportConfigApplyService : IXrayTransportConfigApplyService
{
    private readonly IXrayServerConfigBuilder _configBuilder;
    private readonly IXrayConfigFileService _configFile;
    private readonly IXrayRuntimeService _runtime;
    private readonly IXrayHealthCheckService _healthCheck;
    private readonly ILogger<XrayTransportConfigApplyService> _log;

    public XrayTransportConfigApplyService(
        IXrayServerConfigBuilder configBuilder,
        IXrayConfigFileService configFile,
        IXrayRuntimeService runtime,
        IXrayHealthCheckService healthCheck,
        ILogger<XrayTransportConfigApplyService> log)
    {
        _configBuilder = configBuilder;
        _configFile = configFile;
        _runtime = runtime;
        _healthCheck = healthCheck;
        _log = log;
    }

    public async Task<AgentTransportApplyResult?> ApplyAsync(
        ApplyServerTransportRequestDto request,
        string payloadHash,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        LogApplyStarted(request, payloadHash);

        string configJson;
        try
        {
            configJson = _configBuilder.BuildJson(request);
        }
        catch (ConfigBuildRejectedException ex)
        {
            return AgentTransportApplyResult.Rejected(
                AgentTransportApplyPhase.ConfigBuild,
                ex.Code,
                ex.Message,
                payloadHash);
        }
        catch (System.Exception ex)
        {
            _log.LogError(
                ex,
                "Xray config build failed OperationId={OperationId} ServerId={ServerId}",
                request.OperationId,
                request.ServerId);
            return AgentTransportApplyResult.Failed(
                AgentTransportApplyPhase.ConfigBuild,
                "config_build_failed",
                "Unable to build Xray transport configuration.",
                payloadHash);
        }

        XrayConfigBackupInfo? backup = null;
        AgentTransportApplyPhase failurePhase = AgentTransportApplyPhase.ConfigWrite;

        try
        {
            backup = await _configFile.ReplaceConfigAsync(configJson, ct);
            failurePhase = AgentTransportApplyPhase.XrayReload;
            await _runtime.RestartAsync(ct);
            failurePhase = AgentTransportApplyPhase.HealthCheck;
            await _healthCheck.EnsureHealthyAsync(ct);

            LogApplySucceeded(request, payloadHash);
            return null;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (System.Exception ex)
        {
            _log.LogError(
                ex,
                "Xray transport apply failed OperationId={OperationId} ServerId={ServerId} Phase={Phase}",
                request.OperationId,
                request.ServerId,
                failurePhase);

            var rollbackAttempted = false;
            var rollbackSucceeded = false;

            if (backup is not null)
            {
                rollbackAttempted = true;
                rollbackSucceeded = await TryRollbackAsync(backup, request, payloadHash, CancellationToken.None);
            }

            if (rollbackAttempted && !rollbackSucceeded)
            {
                return AgentTransportApplyResult.Failed(
                    AgentTransportApplyPhase.Rollback,
                    "rollback_failed",
                    "Transport configuration apply failed and rollback did not complete.",
                    payloadHash,
                    rollbackAttempted: true,
                    rollbackSucceeded: false);
            }

            return AgentTransportApplyResult.Failed(
                failurePhase,
                PhaseToErrorCode(failurePhase),
                PhaseToErrorMessage(failurePhase),
                payloadHash,
                rollbackAttempted: rollbackAttempted,
                rollbackSucceeded: rollbackSucceeded);
        }
    }

    private async Task<bool> TryRollbackAsync(
        XrayConfigBackupInfo backup,
        ApplyServerTransportRequestDto request,
        string payloadHash,
        CancellationToken ct)
    {
        try
        {
            LogRollbackStarted(request, payloadHash);

            await _configFile.RestoreBackupAsync(backup, ct);
            await _runtime.RestartAsync(ct);
            await _healthCheck.EnsureHealthyAsync(ct);

            LogRollbackSucceeded(request, payloadHash);
            return true;
        }
        catch (System.Exception rollbackEx)
        {
            _log.LogError(
                rollbackEx,
                "Xray config rollback failed OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
                request.OperationId,
                request.ServerId,
                request.ActivationId,
                payloadHash);
            return false;
        }
    }

    private static string PhaseToErrorCode(AgentTransportApplyPhase phase)
        => phase switch
        {
            AgentTransportApplyPhase.ConfigWrite => "config_write_failed",
            AgentTransportApplyPhase.XrayReload => "xray_reload_failed",
            AgentTransportApplyPhase.HealthCheck => "health_check_failed",
            _ => "config_apply_failed"
        };

    private static string PhaseToErrorMessage(AgentTransportApplyPhase phase)
        => phase switch
        {
            AgentTransportApplyPhase.ConfigWrite => "Unable to write Xray configuration.",
            AgentTransportApplyPhase.XrayReload => "Unable to reload Xray.",
            AgentTransportApplyPhase.HealthCheck => "Xray health check failed after apply.",
            _ => "Transport configuration apply failed."
        };

    private void LogApplyStarted(ApplyServerTransportRequestDto request, string payloadHash)
    {
        _log.LogInformation(
            "Starting Xray transport apply OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
    }

    private void LogApplySucceeded(ApplyServerTransportRequestDto request, string payloadHash)
    {
        _log.LogInformation(
            "Xray transport apply succeeded OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
    }

    private void LogRollbackStarted(ApplyServerTransportRequestDto request, string payloadHash)
    {
        _log.LogWarning(
            "Trying to rollback Xray config OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
    }

    private void LogRollbackSucceeded(ApplyServerTransportRequestDto request, string payloadHash)
    {
        _log.LogWarning(
            "Xray config rollback succeeded OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
    }
}
