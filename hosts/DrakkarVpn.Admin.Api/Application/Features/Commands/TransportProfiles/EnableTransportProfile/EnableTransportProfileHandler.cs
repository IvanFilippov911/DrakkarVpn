using DrakkarVpn.Servers.Application.Abstractions.Services;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.EnableTransportProfile;

public sealed class EnableTransportProfileHandler : IRequestHandler<EnableTransportProfileRequest, bool>
{
    private readonly ITransportProfilesManagementService _service;

    public EnableTransportProfileHandler(ITransportProfilesManagementService service)
        => _service = service;

    public Task<bool> Handle(EnableTransportProfileRequest command, CancellationToken ct)
        => _service.EnableAsync(command.ProfileId, ct);
}
