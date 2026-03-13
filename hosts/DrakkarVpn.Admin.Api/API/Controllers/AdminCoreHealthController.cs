using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreHealth;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreHealthHistory;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.HealthSystem.GetCoreHealth;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.HealthSystem.GetCoreHealthHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Route("api/admin")]
public sealed class AdminCoreHealthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCoreHealthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("core/health")]
    [ProducesResponseType(typeof(CoreHealthDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CoreHealthDto>> GetCoreHealth(CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetCoreHealthQuery(), ct);
        return Ok(dto);
    }

    [HttpGet("core/health/history")]
    [ProducesResponseType(typeof(IReadOnlyList<CoreHealthHistoryPointDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CoreHealthHistoryPointDto>>> GetCoreHealthHistory(
        [FromQuery] int? limit,
        CancellationToken ct = default)
    {
        var items = await _mediator.Send(new GetCoreHealthHistoryQuery(limit), ct);
        return Ok(items);
    }
}