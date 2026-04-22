using DrakkarVpn.Servers.Application.Abstractions;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.DisableTransportProfile;

public sealed record DisableTransportProfileRequest(Guid ProfileId) : IServersCommand<bool>;
