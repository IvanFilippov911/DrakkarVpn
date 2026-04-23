using DrakkarVpn.Agent.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.API.Controllers;

[ApiController]
[Route("server-transport")]
public sealed class ServerTransportController : ControllerBase
{
    [HttpPost("apply")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Apply([FromBody] ApplyServerTransportRequestDto? body)
    {
        if (body is null || body.ServerId == Guid.Empty)
            return BadRequest(new { code = "bad_request", message = "body required" });

        // Inbound: apply XRay / process config (wired separately).
        return NoContent();
    }
}
