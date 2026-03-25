using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Services;
using DrakkarVpn.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.ConnectDevice;

public sealed class ConnectDeviceCommandHandler
    : IRequestHandler<ConnectDeviceRequest, ConnectDeviceResultDto>
{
    private readonly IConnectDeviceService _svc;

    public ConnectDeviceCommandHandler(IConnectDeviceService svc) => _svc = svc;

    public async Task<ConnectDeviceResultDto> Handle(ConnectDeviceRequest req, CancellationToken ct)
    {
        var input = new ConnectDeviceInput(
            InitData: req.InitData,
            DeviceName: req.DeviceName,
            Platform: req.Platform,
            ExistingDeviceId: req.ExistingDeviceId
        );

        var result = await _svc.ConnectAsync(input, ct);

        return new ConnectDeviceResultDto(
            AccessToken: result.AccessToken,
            DeviceId: result.DeviceId
        );
    }
}