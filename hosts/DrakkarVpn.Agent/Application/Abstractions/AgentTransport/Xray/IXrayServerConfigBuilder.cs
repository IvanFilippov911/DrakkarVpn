using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IXrayServerConfigBuilder
{
    string BuildJson(ApplyServerTransportRequestDto request);
}