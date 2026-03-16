using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Overview;
using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminOverview;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetServersOverview;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetUsersOverview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Controllers;


[ApiController]
[Authorize]
[Route("api/admin")]
public sealed class AdminOverviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminOverviewController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("overview")]
    [ProducesResponseType(typeof(AdminOverviewApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminOverviewApiResponse>> GetOverview(CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetAdminOverviewQuery(), ct);
        return Ok(dto.ToApiResponse());
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