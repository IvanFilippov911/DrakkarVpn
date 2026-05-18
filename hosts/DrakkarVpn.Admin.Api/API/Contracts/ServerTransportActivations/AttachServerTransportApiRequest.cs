namespace DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;

public sealed record AttachServerTransportApiRequest(
    IReadOnlyList<AttachServerTransportApiItem> Profiles,
    Guid? ActivateProfileId);

public sealed record AttachServerTransportApiItem(
    Guid TransportProfileId,
    string RealityPublicKey,
    int LocalPriority);
