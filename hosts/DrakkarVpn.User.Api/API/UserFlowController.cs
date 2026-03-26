using DrakkarVpn.Core.Api.Application.Features.Commands.PurchaseSubscription;
using DrakkarVpn.Core.Api.Application.Features.Queries.GetPeerByUuid;
using DrakkarVpn.Core.Api.Application.Features.Queries.GetPeerProvisionJob;
using DrakkarVpn.Core.Api.Modules.Orchestrator.API.Contracts.Response;
using DrakkarVpn.Core.Api.Modules.Orchestrator.API.Contracts.Tariffs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.ConnectDevice;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RegisterUser;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.StartVpnConfigProvisioning;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetActiveTariffs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetCurrentVpnConfig;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetHomeContext;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerByUuid;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetPeerProvisionJob;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure;
using DrakkarVpn.Shared.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.API;

[ApiController]
[Route("api/v1/user")]
public sealed class UserFlowController : ControllerBase
{
    private readonly IMediator _mediator;
    public UserFlowController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResponse>> Register([FromBody] RegisterUserRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegisterRequest(req), ct);
        return Ok(result.ToApiResponse());
    }
    
    
    [HttpPost("devices/connect")]
    public async Task<ActionResult<ConnectDeviceResponse>> ConnectDevice([FromBody] ConnectDeviceRequest body, CancellationToken ct)
    {
        var res = await _mediator.Send(new ConnectDeviceRequest(
            InitData: body.InitData,
            DeviceName: body.DeviceName,
            Platform: body.Platform,
            ExistingDeviceId: body.ExistingDeviceId
        ), ct);

        return Ok(res.ToApiResponse());
    }
    
    [Authorize]
    [HttpGet("home-context")]
    [ProducesResponseType(typeof(HomeContextResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<HomeContextResponse>> GetHomeContext(CancellationToken ct)
    {
        var req = new GetHomeContextRequest(
            TelegramId: User.GetTelegramId(),
            DeviceId: User.GetDeviceId()
        );

        var res = await _mediator.Send(req, ct);
        return Ok(res.ToApiResponse());
    }
    
    
    [Authorize]
    [HttpGet("tariffs")]
    [ProducesResponseType(typeof(IReadOnlyList<UserTariffResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserTariffResponse>>> GetActiveTariffs(CancellationToken ct)
    {
        var items = await _mediator.Send(new GetActiveTariffsQuery(), ct);
        return Ok(items.ToUserApiResponse());
    }
    
    
    [Authorize]
    [HttpPost("subscriptions/purchase")]
    public async Task<ActionResult<Guid>> PurchaseSubscription([FromBody] PurchaseSubscriptionRequest body, CancellationToken ct)
        => Ok(await _mediator.Send(body, ct));
    

    [Authorize]
    [HttpGet("vpn/config/current")]
    [ProducesResponseType(typeof(StatusVpnConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusVpnConfigResponse), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<StatusVpnConfigResponse>> GetCurrentVpnConfig(CancellationToken ct)
    {
        var req = new GetCurrentVpnConfigRequest(
            TelegramId: User.GetTelegramId(),
            DeviceId: User.GetDeviceId()
        );

        var dto = await _mediator.Send(req, ct);
        var res = dto.ToApiResponse();
        return dto.Status switch
        {
            VpnConfigStatusDto.Ready => Ok(res),
            VpnConfigStatusDto.Pending => Accepted(res),
            VpnConfigStatusDto.NotStarted => Ok(res),
            _ => Ok(res)
        };
    }

    [Authorize]
    [HttpPost("vpn/config/provision")]
    [ProducesResponseType(typeof(StatusVpnConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(StatusVpnConfigResponse), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<StatusVpnConfigResponse>> ProvisionVpnConfig(CancellationToken ct)
    {
        var req = new StartPeerProvisioningRequest(
            TelegramId: User.GetTelegramId(),
            DeviceId: User.GetDeviceId()
        );

        var dto = await _mediator.Send(req, ct);
        var res = dto.ToApiResponse();
        return dto.Status == VpnConfigStatusDto.Ready ? Ok(res) : Accepted(res);
    }
    
    [Authorize]
    [HttpGet("vpn/config/provision/{jobId:guid}")]
    [ProducesResponseType(typeof(PeerProvisionJobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PeerProvisionJobResponse>> GetPeerProvisionJob([FromRoute] Guid jobId, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetPeerProvisionJobQuery(jobId), ct);
        return dto is null ? NotFound() : Ok(dto.ToApiResponse());
    }
    
    [HttpGet("access/{peerUuid:guid}")] 
    public async Task<IActionResult> GetPeerConfig([FromRoute] Guid peerUuid, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPeerByUuidQuery(peerUuid), ct);
        if (result is null) return NotFound("Пир не найден");
        return Content(result, "text/plain");
    }
}