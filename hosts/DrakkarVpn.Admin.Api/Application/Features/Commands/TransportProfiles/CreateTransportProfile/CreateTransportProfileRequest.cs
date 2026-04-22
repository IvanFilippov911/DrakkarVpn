using DrakkarVpn.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.CreateTransportProfile;

public sealed record CreateTransportProfileRequest(CreateTransportProfileDto Data)
    : IServersCommand<Guid>;
