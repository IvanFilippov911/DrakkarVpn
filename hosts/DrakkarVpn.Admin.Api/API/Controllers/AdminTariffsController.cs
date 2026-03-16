using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Tariffs;
using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Admin.Api.Application.Features.Commands.Tariffs.AdminCreateTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminDisableTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminEnableTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminUpdateTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetActiveTariffs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetTariffById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Controllers;

[ApiController]
[Authorize(Policy = AdminPolicies.TariffsManage)]
[Route("api/admin/tariffs")]
public sealed class AdminTariffsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminTariffsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTariffApiRequest body, CancellationToken ct)
    {
        var cmd = body.ToCommand();
        var id = await _mediator.Send(cmd, ct);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTariffApiRequest body, CancellationToken ct)
    {
        var cmd = body.ToCommand(id);
        await _mediator.Send(cmd, ct);

        return NoContent();
    }

    [HttpPatch("{id:guid}/enable")]
    public async Task<IActionResult> Enable(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new AdminEnableTariffCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/disable")]
    public async Task<IActionResult> Disable(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new AdminDisableTariffCommand(id), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TariffApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<TariffApiResponse>> GetById(Guid id, CancellationToken ct)
    {
        var tariff = await _mediator.Send(new AdminGetTariffByIdQuery(id), ct);
        return tariff is not null ? Ok(tariff.ToApiResponse()) : NotFound();
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IReadOnlyList<TariffApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TariffApiResponse>>> GetActive(CancellationToken ct)
    {
        var items = await _mediator.Send(new AdminGetActiveTariffsQuery(), ct);
        return Ok(items.ToApiResponse());
    }
}