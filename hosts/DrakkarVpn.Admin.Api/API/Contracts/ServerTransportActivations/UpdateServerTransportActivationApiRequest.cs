namespace DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;

public sealed record UpdateServerTransportActivationApiRequest(
    string RealityPublicKey,
    int LocalPriority);
