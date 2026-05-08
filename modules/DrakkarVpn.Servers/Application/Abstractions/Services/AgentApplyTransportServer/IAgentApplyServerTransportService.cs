using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IAgentApplyServerTransportService
{ Task<IReadOnlyDictionary<Guid, AgentApplyResult>> ApplyAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        CancellationToken ct);
}