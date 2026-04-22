namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

public sealed record AttachServerTransportProfileItemInput(
    Guid TransportProfileId,
    string RealityPublicKey,
    int LocalPriority);
