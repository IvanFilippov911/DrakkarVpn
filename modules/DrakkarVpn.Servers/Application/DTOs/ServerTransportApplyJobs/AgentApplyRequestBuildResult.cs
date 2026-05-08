namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record AgentApplyRequestBuildResult(
    IReadOnlyList<PreparedAgentTransportApplyRequest> Requests,
    IReadOnlyDictionary<Guid, AgentApplyResult> Failed);
