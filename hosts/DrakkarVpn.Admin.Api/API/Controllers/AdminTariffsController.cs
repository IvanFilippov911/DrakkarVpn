using DrakkarVpn.Admin.Api.Application.Features.Commands.Tariffs.AdminCreateTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminDisableTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminEnableTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminUpdateTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetActiveTariffs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetTariffById;
using DrakkarVpn.Shared.Tariffs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Controllers;

[ApiController]
[Route("api/admin/tariffs")]
public sealed class AdminTariffsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminTariffsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTariffBody body, CancellationToken ct)
    {
        var id = await _mediator.Send(
            new AdminCreateTariffCommand(body.Name, TimeSpan.FromDays(body.DurationDays), body.Price, body.DefaultMaxDevices),
            ct);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTariffBody body, CancellationToken ct)
    {
        await _mediator.Send(
            new AdminUpdateTariffCommand(id, body.Name, TimeSpan.FromDays(body.DurationDays), body.Price, body.DefaultMaxDevices),
            ct);

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
    public async Task<ActionResult<TariffDto>> GetById(Guid id, CancellationToken ct)
    {
        var tariff = await _mediator.Send(new AdminGetTariffByIdQuery(id), ct);
        return tariff is not null ? Ok(tariff) : NotFound();
    }

    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<TariffDto>>> GetActive(CancellationToken ct)
        => Ok(await _mediator.Send(new AdminGetActiveTariffsQuery(), ct));

    public sealed record CreateTariffBody(string Name, int DurationDays, decimal Price, int DefaultMaxDevices);
    public sealed record UpdateTariffBody(string Name, int DurationDays, decimal Price, int DefaultMaxDevices);
}