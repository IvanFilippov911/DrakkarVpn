namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportApplyJobs;

public sealed record ServerTransportDesiredStateDto(
    Guid? DesiredTransportActivationId,
    long DesiredTransportVersion);