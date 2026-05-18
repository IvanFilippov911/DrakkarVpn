using DrakkarVpn.Servers.Domain.Enums.TransportProfile;

namespace DrakkarVpn.Admin.Api.Application.DTOs.ServerTransportActivations;

public sealed record ServerTransportApplyJobStatusDto(
    Guid JobId,
    Guid ServerId,
    Guid ActivationId,
    long TargetTransportVersion,
    ServerTransportApplyJobStatus State,
    string? LastErrorCode,
    string? LastErrorMessage);
