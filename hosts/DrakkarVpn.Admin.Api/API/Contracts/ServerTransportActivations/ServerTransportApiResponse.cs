using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;

public sealed record ServerTransportApiResponse(
    Guid ActivationId,
    Guid ServerId,
    Guid TransportProfileId,
    string TransportProfileName,
    TransportActivationStatus Status,
    int LocalPriority,
    string RealityPublicKey,
    DateTimeOffset? ActivatedAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    int Version);
