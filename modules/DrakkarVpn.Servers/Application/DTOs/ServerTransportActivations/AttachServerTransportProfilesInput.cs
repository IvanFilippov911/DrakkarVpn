namespace DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

public sealed record AttachServerTransportProfilesInput(
    Guid ServerId,
    IReadOnlyList<AttachServerTransportProfileItemInput> Profiles,
    Guid? ActivateProfileId);
