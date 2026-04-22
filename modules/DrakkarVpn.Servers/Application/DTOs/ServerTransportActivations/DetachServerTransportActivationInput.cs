namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

public sealed record DetachServerTransportActivationInput(
    Guid ServerId,
    Guid ActivationId);
