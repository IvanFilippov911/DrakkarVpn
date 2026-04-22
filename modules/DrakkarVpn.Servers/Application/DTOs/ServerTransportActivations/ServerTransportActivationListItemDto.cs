using DrakkarVpn.Servers.Domain.Enums;

namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

public sealed record ServerTransportActivationListItemDto(
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
