using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record PreparedAgentTransportApplyRequest(
    Guid JobId,
    string AgentBaseUrl,
    AgentApplyServerTransportRequest Request);
