using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions.AgentTransport;

public interface IAgentTransportPayloadHashService
{
    string Calculate(ApplyServerTransportRequestDto request);
}
