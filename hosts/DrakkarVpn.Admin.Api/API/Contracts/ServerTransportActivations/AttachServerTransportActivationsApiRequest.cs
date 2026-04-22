namespace DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;

public sealed record AttachServerTransportActivationsApiRequest(
    IReadOnlyList<AttachServerTransportActivationApiItem> Profiles,
    Guid? ActivateProfileId);

public sealed record AttachServerTransportActivationApiItem(
    Guid TransportProfileId,
    string RealityPublicKey,
    int LocalPriority);
