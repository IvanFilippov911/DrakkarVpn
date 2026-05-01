using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Application.Feature.Commands.ApplyServerTransport;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.API.Controllers;

[ApiController]
[Route("server-transport")]
public sealed class ServerTransportController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServerTransportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("apply")]
    [ProducesResponseType(typeof(AgentTransportApplyWireResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AgentTransportApplyWireResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AgentTransportApplyWireResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Apply([FromBody] ApplyServerTransportRequestDto? body, CancellationToken ct)
    {
        if (body is null)
            return BadRequest(new { code = "bad_request", message = "body required" });

        var result = await _mediator.Send(new ApplyServerTransportCommand(body), ct);

        var wire = AgentTransportApplyWireMapper.ToWireResponse(result);

        if (result is { IsSuccess: true })
            return Ok(wire);

        if (result.IsClientError)
            return BadRequest(wire);

        return StatusCode(StatusCodes.Status500InternalServerError, wire);
    }
}
