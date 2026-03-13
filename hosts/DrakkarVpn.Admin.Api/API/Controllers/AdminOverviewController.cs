using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminOverview;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetServersOverview;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetUsersOverview;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Controllers;


[ApiController]
[Route("api/admin")]
public sealed class AdminOverviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminOverviewController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("overview")]
    [ProducesResponseType(typeof(AdminOverviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminOverviewDto>> GetOverview(CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetAdminOverviewQuery(), ct);
        return Ok(dto);
    }
    
    [HttpGet("overview/servers")]
    [ProducesResponseType(typeof(ServersWithTrafficOverviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServersWithTrafficOverviewDto>> GetServersOverview(CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetServersOverviewQuery(), ct);
        return Ok(dto);
    }
    
    [HttpGet("overview/users")]
    [ProducesResponseType(typeof(UsersOverviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UsersOverviewDto>> GetUsersOverview(CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetUsersOverviewQuery(), ct);
        return Ok(dto);
    }
}