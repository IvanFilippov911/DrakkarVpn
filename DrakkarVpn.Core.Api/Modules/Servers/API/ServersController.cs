using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.BenchmarkServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.DeleteServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.RegisterServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerById;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistory;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistoryLast;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Servers.API;

[ApiController]
[Route("api/servers")]
public sealed class ServersController : ControllerBase
{
    private readonly IMediator _mediator;
    public ServersController(IMediator mediator) => _mediator = mediator;
    
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GetServerDto>>> Get([FromQuery] string? region, [FromQuery] string? status, CancellationToken ct)
        => Ok(await _mediator.Send(new GetServersRequest(region, status), ct));

    
    [HttpPost]
    public async Task<ActionResult<Guid>> Register([FromBody] RegisterServerRequest body, CancellationToken ct)
    {
        var id = await _mediator.Send(body, ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteServerRequest(id), ct);
        return ok ? NoContent() : NotFound();
    }
    
    [HttpPost("{id:guid}/benchmark")]
    public async Task<ActionResult<BenchmarkResultDto>> Benchmark(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new BenchmarkServerRequest(id), ct);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<ServerMetricsHistoryDto>>> GetHistory(
        Guid id, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetServerHistoryRequest(id, from, to), ct);
        return Ok(dto);
    }

    [HttpGet("{id:guid}/history/last")]
    public async Task<ActionResult<IReadOnlyList<ServerMetricsHistoryDto>>> GetHistoryLast(
        Guid id, [FromQuery] int minutes = 1440, CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetServerHistoryLastRequest(id, minutes), ct);
        return Ok(dto);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetServerByIdRequest(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
    
    
}