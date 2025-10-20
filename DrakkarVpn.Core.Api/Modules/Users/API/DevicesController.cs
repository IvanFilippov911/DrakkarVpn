using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ConnectDeviceRequest = DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ConnectDevice.ConnectDeviceRequest;

namespace DrakkarVpn.Core.Api.Modules.Users.API;

[ApiController]
[Route("api/users/devices")]
public sealed class DevicesController : ControllerBase
{
    private readonly IMediator _mediator;
    public DevicesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("connect")]
    public async Task<ActionResult<ConnectDeviceResponse>> Connect([FromBody] ConnectDeviceRequest body, CancellationToken ct)
    {
        var res = await _mediator.Send(new ConnectDeviceRequest(
            InitData:   body.InitData,
            DeviceName: body.DeviceName,
            Platform:   body.Platform,
            ExistingDeviceId:  body.ExistingDeviceId
        ), ct);

        return Ok(res);
    }
}