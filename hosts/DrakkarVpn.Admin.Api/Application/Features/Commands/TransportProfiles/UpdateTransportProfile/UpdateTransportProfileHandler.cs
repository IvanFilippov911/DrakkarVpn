using DrakkarVpn.Servers.Application.Abstractions.Services;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.UpdateTransportProfile;

public sealed class UpdateTransportProfileHandler : IRequestHandler<UpdateTransportProfileRequest, bool>
{
    private readonly ITransportProfilesManagementService _service;

    public UpdateTransportProfileHandler(ITransportProfilesManagementService service)
        => _service = service;

    public Task<bool> Handle(UpdateTransportProfileRequest command, CancellationToken ct)
        => _service.UpdateAsync(command.ProfileId, command.Data, ct);
}
