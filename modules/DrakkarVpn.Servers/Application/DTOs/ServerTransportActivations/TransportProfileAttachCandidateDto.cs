namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

public sealed record TransportProfileAttachCandidateDto(
    Guid TransportProfileId,
    bool IsEnabled);
