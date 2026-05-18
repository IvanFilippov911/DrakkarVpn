namespace DrakkarVpn.Servers.Application.Abstractions.Repositories;

public readonly record struct ServerTransportApplyJobMarkFailedBatchRow(
    Guid JobId,
    string ErrorCode,
    string? ErrorMessage);

public readonly record struct ServerTransportApplyJobRescheduleBatchRow(
    Guid JobId,
    int NewAttempt,
    DateTime NextAttemptAtUtc,
    string ErrorCode,
    string? ErrorMessage);
