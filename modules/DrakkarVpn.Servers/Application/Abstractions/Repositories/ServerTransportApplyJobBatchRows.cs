namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

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
