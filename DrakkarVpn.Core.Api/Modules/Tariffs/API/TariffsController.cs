using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.CreateTariff;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.DisableTariff;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.EnableTariff;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.UpdateTariff;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Queries.GetActiveTariffs;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Queries.GetTariffById;
using DrakkarVpn.Shared.Tariffs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.API;

[ApiController]
[Route("api/admin/tariffs")]
public sealed class TariffsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TariffsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTariffRequest req, CancellationToken ct)
    {
        var id = await _mediator.Send(req, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTariffBody body, CancellationToken ct)
    {
        await _mediator.Send(new UpdateTariffRequest(id, body.Name, body.Price, body.Duration), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/enable")]
    public async Task<IActionResult> Enable(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new EnableTariffRequest(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/disable")]
    public async Task<IActionResult> Disable(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DisableTariffRequest(id), ct);
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TariffDto>> GetById(Guid id, CancellationToken ct)
    {
        var tariff = await _mediator.Send(new GetTariffByIdRequest(id), ct);
        return tariff is not null ? Ok(tariff) : NotFound();
    }

    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyList<TariffDto>>> GetActive(CancellationToken ct)
        => Ok(await _mediator.Send(new GetActiveTariffsRequest(), ct));

    public sealed record UpdateTariffBody(string Name, TimeSpan Duration, decimal Price);
}