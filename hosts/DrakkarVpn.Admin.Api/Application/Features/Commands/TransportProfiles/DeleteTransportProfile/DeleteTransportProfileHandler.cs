using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.DeleteTransportProfile;

public sealed class DeleteTransportProfileHandler
    : IRequestHandler<DeleteTransportProfileRequest, DeleteTransportProfileResult>
{
    private readonly ITransportProfilesManagementService _service;

    public DeleteTransportProfileHandler(ITransportProfilesManagementService service)
        => _service = service;

    public Task<DeleteTransportProfileResult> Handle(DeleteTransportProfileRequest command, CancellationToken ct)
        => _service.DeleteAsync(command.ProfileId, ct);
}
