using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IXrayTransportConfigApplyService
{
    /// <returns>null when the xray pipeline succeeded; otherwise Rejected or Failed.</returns>
    Task<AgentTransportApplyResult?> ApplyAsync(
        ApplyServerTransportRequestDto request,
        string payloadHash,
        CancellationToken ct);
}
