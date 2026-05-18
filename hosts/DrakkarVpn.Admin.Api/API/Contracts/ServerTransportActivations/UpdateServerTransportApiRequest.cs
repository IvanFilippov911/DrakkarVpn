namespace DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;

public sealed record UpdateServerTransportApiRequest(
    string RealityPublicKey,
    int LocalPriority);
