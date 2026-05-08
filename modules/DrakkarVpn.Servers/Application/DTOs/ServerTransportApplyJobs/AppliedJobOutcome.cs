namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record AppliedJobOutcome(
    Guid JobId,
    Guid ServerId,
    Guid ActivationId,
    long Version);
