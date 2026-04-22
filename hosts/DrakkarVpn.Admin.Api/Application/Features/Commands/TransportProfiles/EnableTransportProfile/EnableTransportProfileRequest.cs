using DrakkarVpn.Servers.Application.Abstractions;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.EnableTransportProfile;

public sealed record EnableTransportProfileRequest(Guid ProfileId) : IServersCommand<bool>;
