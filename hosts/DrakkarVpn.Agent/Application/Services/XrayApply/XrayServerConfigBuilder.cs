using DrakkarVpn.Agent.Application.Abstractions.AgentTransport;
using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Services.XrayApply;

public sealed class XrayServerConfigBuilder : IXrayServerConfigBuilder
{
    public string BuildJson(ApplyServerTransportRequestDto request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return "{}";
    }
}
