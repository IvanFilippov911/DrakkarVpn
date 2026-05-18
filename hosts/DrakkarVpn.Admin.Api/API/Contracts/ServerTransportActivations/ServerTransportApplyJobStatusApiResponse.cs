using DrakkarVpn.Servers.Domain.Enums.TransportProfile;

namespace DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;

public sealed record ServerTransportApplyJobStatusApiResponse(
    Guid JobId,
    Guid ServerId,
    Guid ActivationId,
    long TargetTransportVersion,
    ServerTransportApplyJobStatus State,
    string? LastErrorCode,
    string? LastErrorMessage);
