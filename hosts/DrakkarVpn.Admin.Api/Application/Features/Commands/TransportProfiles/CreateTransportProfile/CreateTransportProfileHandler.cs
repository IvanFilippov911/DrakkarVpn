using DrakkarVpn.Servers.Application.Abstractions.Services;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.CreateTransportProfile;

public sealed class CreateTransportProfileHandler
    : IRequestHandler<CreateTransportProfileRequest, Guid>
{
    private readonly ITransportProfilesManagementService _service;

    public CreateTransportProfileHandler(ITransportProfilesManagementService service)
        => _service = service;

    public Task<Guid> Handle(CreateTransportProfileRequest command, CancellationToken ct)
        => _service.CreateAsync(command.Data, ct);
}
