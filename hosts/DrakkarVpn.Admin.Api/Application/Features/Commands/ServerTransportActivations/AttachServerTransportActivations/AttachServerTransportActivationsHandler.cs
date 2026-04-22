using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.AttachServerTransportActivations;

public sealed class AttachServerTransportActivationsHandler
    : IRequestHandler<AttachServerTransportActivationsRequest, Unit>
{
    private readonly IServerTransportActivationManagementService _service;

    public AttachServerTransportActivationsHandler(IServerTransportActivationManagementService service)
        => _service = service;

    public async Task<Unit> Handle(AttachServerTransportActivationsRequest command, CancellationToken ct)
    {
        var input = new AttachServerTransportProfilesInput(
            ServerId: command.ServerId,
            Profiles: command.Profiles
                .Select(x => new AttachServerTransportProfileItemInput(
                    x.TransportProfileId,
                    x.RealityPublicKey,
                    x.LocalPriority))
                .ToList(),
            ActivateProfileId: command.ActivateProfileId);

        await _service.AttachProfilesAsync(input, ct);
        return Unit.Value;
    }
}
