using DrakkarVpn.Servers.Application.Abstractions.Services;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.DisableTransportProfile;

public sealed class DisableTransportProfileHandler : IRequestHandler<DisableTransportProfileRequest, bool>
{
    private readonly ITransportProfilesManagementService _service;

    public DisableTransportProfileHandler(ITransportProfilesManagementService service)
        => _service = service;

    public Task<bool> Handle(DisableTransportProfileRequest command, CancellationToken ct)
        => _service.DisableAsync(command.ProfileId, ct);
}
