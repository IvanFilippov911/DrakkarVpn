using DrakkarVpn.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.UpdateTransportProfile;

public sealed record UpdateTransportProfileRequest(
    Guid ProfileId,
    UpdateTransportProfileDto Data
) : IServersCommand<bool>;
