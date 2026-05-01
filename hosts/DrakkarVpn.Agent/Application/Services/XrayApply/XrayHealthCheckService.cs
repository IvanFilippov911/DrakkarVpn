using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Services.XrayApply;

public interface IXrayTransportConfigApplyService
{
    Task ApplyAsync(
        ApplyServerTransportRequestDto request,
        string payloadHash,
        CancellationToken ct);
}