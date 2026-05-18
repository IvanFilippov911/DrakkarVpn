using DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;
using DrakkarVpn.Servers.Infrastructure.EF.Entities;

namespace DrakkarVpn.Servers.Application.Mappers;

public static class ServerTransportApplyJobMappers
{
    public static ServerTransportApplyJobDto ToDto(this ServerTransportApplyJob row)
        => new(
            row.JobId,
            row.ServerId,
            row.ActivationId,
            row.TargetTransportVersion,
            row.State,
            row.LeaseUntilUtc,
            row.LeaseOwner,
            row.Attempt,
            row.MaxAttempt,
            row.NextAttemptUtc,
            row.LastErrorCode,
            row.LastErrorMessage,
            row.CreatedAtUtc,
            row.UpdatedAtUtc,
            row.CompletedAtUtc);
}
