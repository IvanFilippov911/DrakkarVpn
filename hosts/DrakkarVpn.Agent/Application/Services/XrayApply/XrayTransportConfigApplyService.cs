using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Services.XrayApply;

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

    public async Task ApplyAsync(
        ApplyServerTransportRequestDto request,
        string payloadHash,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);

        LogApplyStarted(request, payloadHash);

        var configJson = _configBuilder.BuildJson(request);

        XrayConfigBackupInfo? backup = null;
        try
        {
            backup = await _configFile.ReplaceConfigAsync(configJson, ct);
            await _runtime.RestartAsync(ct);
            await _healthCheck.EnsureHealthyAsync(ct);

            LogApplySucceeded(request, payloadHash);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            if (backup is not null)
                await TryRollbackAsync(backup, request, payloadHash, CancellationToken.None);

            throw;
        }
    }
    
    private async Task TryRollbackAsync(
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
        }
    }

    private void LogApplyStarted(
        ApplyServerTransportRequestDto request,
        string payloadHash)
    {
        _log.LogInformation(
            "Starting Xray transport apply OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
    }

    private void LogApplySucceeded(
        ApplyServerTransportRequestDto request,
        string payloadHash)
    {
        _log.LogInformation(
            "Xray transport apply succeeded OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
    }

    private void LogRollbackStarted(
        ApplyServerTransportRequestDto request,
        string payloadHash)
    {
        _log.LogWarning(
            "Trying to rollback Xray config OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
    }

    private void LogRollbackSucceeded(
        ApplyServerTransportRequestDto request,
        string payloadHash)
    {
        _log.LogWarning(
            "Xray config rollback succeeded OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);
    }
}