using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IServerTransportApplyJobProcessor
{
    Task ProcessAsync(IReadOnlyCollection<ServerTransportApplyJobDto> jobs, string leaseOwner, CancellationToken ct);
}