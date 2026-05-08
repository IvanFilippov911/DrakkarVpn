using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportProfileApply;

public interface IServerTransportAppliedRecorder
{
    Task<int> RecordAsync(
        IReadOnlyCollection<AppliedJobOutcome> applied,
        string leaseOwner,
        DateTime utcNow,
        CancellationToken ct);
}
