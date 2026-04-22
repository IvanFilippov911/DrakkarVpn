namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

public sealed record ActivateServerTransportActivationInput(
    Guid ServerId,
    Guid ActivationId);
