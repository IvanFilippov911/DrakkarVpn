using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Application.DTOs.Enums;
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
        {
            var rejected = AgentTransportApplyResult.Rejected(
                AgentTransportApplyPhase.Validation,
                "body_required",
                "body required");
            return BadRequest(AgentTransportApplyWireMapper.ToWireResponse(rejected));
        }

        var result = await _mediator.Send(new ApplyServerTransportCommand(body), ct);
        var wire = AgentTransportApplyWireMapper.ToWireResponse(result);

        if (result.IsSuccess)
            return Ok(wire);

        if (result.IsRejected)
            return BadRequest(wire);

        return StatusCode(StatusCodes.Status500InternalServerError, wire);
    }
}
