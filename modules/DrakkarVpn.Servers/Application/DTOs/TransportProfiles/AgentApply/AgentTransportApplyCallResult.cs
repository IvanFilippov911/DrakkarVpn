using DrakkarVpn.Servers.Application.DTOs.TransportProfiles.AgentApply;

namespace DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

public sealed record AgentTransportApplyCallResult(
    AgentApplyServerTransportWireResponse? Wire,
    string? TransportErrorCode,
    string? TransportErrorMessage)
{
    public static AgentTransportApplyCallResult FromWire(AgentApplyServerTransportWireResponse wire)
        => new(wire, null, null);

    public static AgentTransportApplyCallResult TransportFailure(string code, string message)
        => new(null, code, message);
}
