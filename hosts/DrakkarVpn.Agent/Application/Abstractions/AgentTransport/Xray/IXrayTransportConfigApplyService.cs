using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IXrayTransportConfigApplyService
{
    Task ApplyAsync(ApplyServerTransportRequestDto request, string payloadHash, CancellationToken ct);
}
