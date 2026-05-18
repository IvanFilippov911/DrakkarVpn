using DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;
using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.AttachServerTransport;

public sealed class AttachServerTransportHandler
    : IRequestHandler<AttachServerTransportRequest, Unit>
{
    private readonly IServerTransportManagementService _service;

    public AttachServerTransportHandler(IServerTransportManagementService service)
        => _service = service;

    public async Task<Unit> Handle(AttachServerTransportRequest command, CancellationToken ct)
    {
        var input = ToInput(command);
        await _service.AttachProfilesAsync(input, ct);

        return Unit.Value;
    }
    
    private static AttachServerTransportProfilesInput ToInput(
        AttachServerTransportRequest request)
    {
        return new AttachServerTransportProfilesInput(
            ServerId: request.ServerId,
            Profiles: request.Profiles
                .Select(item => new AttachServerTransportProfileItemInput(
                    item.TransportProfileId,
                    item.RealityPublicKey,
                    item.LocalPriority))
                .ToList(),
            ActivateProfileId: request.ActivateProfileId);
    }
}
