namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record ServerTransportApplyJobFailure(
    ServerTransportApplyJobDto Job,
    string ErrorCode,
    string? ErrorMessage);
