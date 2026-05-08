using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IAgentApplyServerTransportRequestBuilder
{
    Task<AgentApplyRequestBuildResult> BuildAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        CancellationToken ct);
}