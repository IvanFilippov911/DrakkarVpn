using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.ConnectDevice;

public sealed class ConnectDeviceCommandHandler
    : IRequestHandler<ConnectDeviceRequest, ConnectDeviceResponse>
{
    private readonly IConnectDeviceService _svc;

    public ConnectDeviceCommandHandler(IConnectDeviceService svc) => _svc = svc;

    public async Task<ConnectDeviceResponse> Handle(ConnectDeviceRequest req, CancellationToken ct)
    {
        var input = new ConnectDeviceInput(
            InitData: req.InitData,
            DeviceName: req.DeviceName,
            Platform: req.Platform,
            ExistingDeviceId: req.ExistingDeviceId
        );

        var result = await _svc.ConnectAsync(input, ct);

        return new ConnectDeviceResponse(
            AccessToken: result.AccessToken,
            DeviceId: result.DeviceId
        );
    }
}