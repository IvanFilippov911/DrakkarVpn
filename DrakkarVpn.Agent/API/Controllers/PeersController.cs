using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.Controllers;

[ApiController]
[Route("peers")]
public class PeersController : ControllerBase
{
    private readonly IV2RayService _service;

    public PeersController(IV2RayService V2RayService)
    {
        _service = V2RayService;
    }

    [HttpPost]
    public async Task<ActionResult<RegisterPeerResponseDto>> RegisterPeer(CancellationToken ct)
    {
        var result = await _service.RegisterPeerAsync(ct);
        return Ok(result);
    }

    [HttpDelete("{peerUuid:guid}")]
    public async Task<IActionResult> RevokePeer(Guid peerUuid, CancellationToken ct)
    {
        var success = await _service.RevokePeerAsync(peerUuid, ct);
        return success ? NoContent() : NotFound();
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Guid>>> ListPeers(CancellationToken ct)
    {
        var peers = await _service.GetListPeersAsync(ct);
        return Ok(peers);
    }

}