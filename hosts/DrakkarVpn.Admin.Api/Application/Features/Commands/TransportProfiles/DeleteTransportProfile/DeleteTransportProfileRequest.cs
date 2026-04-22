using DrakkarVpn.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.DeleteTransportProfile;

public sealed record DeleteTransportProfileRequest(Guid ProfileId)
    : IServersCommand<DeleteTransportProfileResult>;
