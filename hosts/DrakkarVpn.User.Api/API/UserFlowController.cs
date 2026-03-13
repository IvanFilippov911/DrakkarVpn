using DrakkarVpn.Core.Api.Application.Features.Commands.PurchaseSubscription;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.ConnectDevice;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.PurchaseSubscription;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RegisterUser;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerByUuid;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerProvisionJob;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure;
using DrakkarVpn.Shared.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API;

[ApiController]
[Route("api/v1/user")]
public sealed class UserFlowController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserFlowController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResponse>> Register([FromBody] RegisterUserRequest req, CancellationToken ct)
        => Ok(await _mediator.Send(new RegisterRequest(req), ct));
    
    [HttpPost("subscriptions/purchase")]
    public async Task<ActionResult<Guid>> PurchaseSubscription([FromBody] PurchaseSubscriptionRequest body, CancellationToken ct)
        => Ok(await _mediator.Send(body, ct));
    
    [HttpPost("devices/connect")]
    public async Task<ActionResult<ConnectDeviceResponse>> ConnectDevice([FromBody] ConnectDeviceRequest body, CancellationToken ct)
    {
        var res = await _mediator.Send(new ConnectDeviceRequest(
            InitData: body.InitData,
            DeviceName: body.DeviceName,
            Platform: body.Platform,
            ExistingDeviceId: body.ExistingDeviceId
        ), ct);

        return Ok(res);
    }
    
    [Authorize]
    [HttpGet("vpn/config")]
    [ProducesResponseType(typeof(GetVpnConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GetVpnConfigResponse), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<GetVpnConfigResponse>> GetVpnConfig(CancellationToken ct)
    {
        var req = new GetVpnConfigRequest(
            TelegramId: User.GetTelegramId(),
            DeviceId: User.GetDeviceId()
        );

        var res = await _mediator.Send(req, ct);
        return res.Status == VpnConfigStatus.Ready ? Ok(res) : Accepted(res);
    }
    
    [Authorize]
    [HttpGet("vpn/provision/{jobId:guid}")]
    [ProducesResponseType(typeof(PeerProvisionJobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PeerProvisionJobResponse>> GetPeerProvisionJob([FromRoute] Guid jobId, CancellationToken ct)
    {
        var res = await _mediator.Send(new GetPeerProvisionJobQuery(jobId), ct);
        return res is null ? NotFound() : Ok(res);
    }
    
    [HttpGet("access/{peerUuid:guid}")] 
    public async Task<IActionResult> GetPeerConfig([FromRoute] Guid peerUuid, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPeerByUuidQuery(peerUuid), ct);
        if (result is null) return NotFound("Пир не найден");
        return Content(result.ConfigRaw, "text/plain");
    }
}