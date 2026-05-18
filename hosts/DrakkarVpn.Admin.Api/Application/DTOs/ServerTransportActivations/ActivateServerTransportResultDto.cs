using DrakkarVpn.Servers.Domain.Enums.TransportProfile;

namespace DrakkarVpn.Admin.Api.Application.DTOs.ServerTransportActivations;

public sealed record ActivateServerTransportResultDto(
    Guid JobId,
    Guid ServerId,
    Guid ActivationId,
    long TargetTransportVersion,
    ServerTransportApplyJobStatus Status,
    string PollUrl);
