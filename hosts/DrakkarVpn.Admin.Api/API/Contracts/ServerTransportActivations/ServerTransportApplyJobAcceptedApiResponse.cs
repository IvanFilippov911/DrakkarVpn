using DrakkarVpn.Servers.Domain.Enums.TransportProfile;

namespace DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;

public sealed record ServerTransportApplyJobAcceptedApiResponse(
    Guid JobId,
    Guid ServerId,
    Guid ActivationId,
    long TargetTransportVersion,
    ServerTransportApplyJobStatus Status,
    string PollUrl);
