using DrakkarVpn.Admin.Api.API.Contracts.NetworkMonitoring;
using DrakkarVpn.Admin.Api.API.Mappings;
using DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.RegisterProbeNode;
using DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.UpdateProbeNode;
using DrakkarVpn.Admin.Api.Application.Features.Commands.NetworkMonitoring.DeleteProbeNode;
using DrakkarVpn.Admin.Api.Application.Features.Queries.NetworkMonitoring.GetProbeNodes;
using DrakkarVpn.AdminAuth.Application.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Authorize]
[Route("api/admin/network-monitoring")]
public sealed class AdminNetworkMonitoringController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminNetworkMonitoringController(IMediator mediator)
        => _mediator = mediator;

    [Authorize(Policy = AdminPolicies.ObservabilityRead)]
    [HttpPost("probe-nodes/register")]
    [ProducesResponseType(typeof(RegisterProbeNodeApiResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RegisterProbeNodeApiResponse>> RegisterProbeNode(
        [FromBody] RegisterProbeNodeApiRequest body,
        CancellationToken ct = default)
    {
        var cmd = body.ToCommand();
        var id = await _mediator.Send(cmd, ct);
        return Created($"/api/admin/network-monitoring/probe-nodes/{id}", new RegisterProbeNodeApiResponse(id));
    }

    [Authorize(Policy = AdminPolicies.ObservabilityRead)]
    [HttpGet("probe-nodes")]
    [ProducesResponseType(typeof(IReadOnlyList<ProbeNodeApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProbeNodeApiResponse>>> GetProbeNodes(
        CancellationToken ct = default)
    {
        var items = await _mediator.Send(new GetProbeNodesQuery(), ct);
        return Ok(items.ToApiResponse());
    }

    [Authorize(Policy = AdminPolicies.ObservabilityRead)]
    [HttpPut("probe-nodes/{probeNodeId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateProbeNode(
        Guid probeNodeId,
        [FromBody] UpdateProbeNodeApiRequest body,
        CancellationToken ct = default)
    {
        var cmd = body.ToCommand(probeNodeId);
        var ok = await _mediator.Send(cmd, ct);
        return ok ? NoContent() : Conflict();
    }

    [Authorize(Policy = AdminPolicies.ObservabilityRead)]
    [HttpDelete("probe-nodes/{probeNodeId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProbeNode(
        Guid probeNodeId,
        CancellationToken ct = default)
    {
        var ok = await _mediator.Send(new DeleteProbeNodeRequest(probeNodeId), ct);
        return ok ? NoContent() : Conflict();
    }
}

