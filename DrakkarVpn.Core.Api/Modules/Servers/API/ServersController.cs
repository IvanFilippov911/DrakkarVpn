using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.DeleteServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.RegisterServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;
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
}