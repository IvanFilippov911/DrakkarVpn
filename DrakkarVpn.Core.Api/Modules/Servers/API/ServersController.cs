using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.ChangeServerStatus;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.RegisterServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpdateServerHealth;
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
    public async Task<ActionResult<IReadOnlyList<GetServersDto>>> Get([FromQuery] string? region, [FromQuery] string? status, CancellationToken ct)
        => Ok(await _mediator.Send(new GetServersRequest(region, status), ct));

    
    [HttpPost]
    public async Task<ActionResult<Guid>> Register([FromBody] RegisterServerRequest body, CancellationToken ct)
    {
        var id = await _mediator.Send(body, ct);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    
    public sealed record ServerChangeStatusBody(ServerStatus Status);

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus([FromRoute] Guid id, [FromBody] ServerChangeStatusBody body, CancellationToken ct)
    {
        var ok = await _mediator.Send(new ChangeServerStatusRequest(id, body.Status), ct);
        return ok ? NoContent() : NotFound();
    }

    
    public sealed record UpdateHealthBody(bool Reachable, int PeersActive);

    [HttpPost("{id:guid}/health")]
    public async Task<IActionResult> UpdateHealth([FromRoute] Guid id, [FromBody] UpdateHealthBody body, CancellationToken ct)
    {
        var ok = await _mediator.Send(new UpdateServerHealthRequest(id, body.Reachable, body.PeersActive), ct);
        return ok ? NoContent() : NotFound();
    }
}