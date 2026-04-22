namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

public sealed record UpdateServerTransportActivationInput(
    Guid ServerId,
    Guid ActivationId,
    string RealityPublicKey,
    int LocalPriority);
