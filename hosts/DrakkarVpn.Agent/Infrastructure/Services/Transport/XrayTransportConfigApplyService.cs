using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Agent.Infrastructure.Services.Transport;

public sealed class XrayTransportConfigApplyService : IXrayTransportConfigApplyService
{
    private readonly ILogger<XrayTransportConfigApplyService> _log;

    public XrayTransportConfigApplyService(ILogger<XrayTransportConfigApplyService> log)
    {
        _log = log;
    }

    public Task ApplyAsync(ApplyServerTransportRequestDto request, string payloadHash, CancellationToken ct)
    {
        _log.LogInformation(
            "Xray transport apply (no-op): OperationId={OperationId} ServerId={ServerId} ActivationId={ActivationId} PayloadHash={PayloadHash}",
            request.OperationId,
            request.ServerId,
            request.ActivationId,
            payloadHash);

        return Task.CompletedTask;
    }
}
