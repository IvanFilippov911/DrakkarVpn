using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;

public interface IServerTransportApplyJobObsoleteGuard
{
    Task<IReadOnlyCollection<ServerTransportApplyJobDto>> FilterActualOrMarkObsoleteAsync(
        IReadOnlyCollection<ServerTransportApplyJobDto> jobs,
        string leaseOwner,
        CancellationToken ct);
}
