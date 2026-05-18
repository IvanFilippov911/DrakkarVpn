namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record ServerTransportApplyJobOutcomeBuckets(
    IReadOnlyList<AppliedJobOutcome> Applied,
    IReadOnlyList<ServerTransportApplyJobFailure> Retryable,
    IReadOnlyList<ServerTransportApplyJobFailure> Permanent);
