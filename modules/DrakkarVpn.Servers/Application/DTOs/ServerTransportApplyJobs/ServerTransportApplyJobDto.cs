using DrakkarVpn.Servers.Domain.Enums.TransportProfile;

namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record ServerTransportApplyJobDto(
    Guid JobId,
    Guid ServerId,
    Guid ActivationId,
    long TargetTransportVersion,
    ServerTransportApplyJobStatus State,
    DateTime? LeaseUntilUtc,
    string? LeaseOwner,
    int Attempt,
    int MaxAttempt,
    DateTime NextAttemptUtc,
    string? LastErrorCode,
    string? LastErrorMessage,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    DateTime? CompletedAtUtc);
