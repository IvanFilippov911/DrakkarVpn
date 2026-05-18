using DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;
using DrakkarVpn.Admin.Api.API.Mappings;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.DeleteServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Queries.ServerTransportActivations.GetServerTransportActivations;
using DrakkarVpn.Admin.Api.Application.Features.Queries.ServerTransportActivations.GetServerTransportApplyJob;
using DrakkarVpn.AdminAuth.Application.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Admin.Api.API.Controllers;

[ApiController]
[Authorize]
[Route("api/admin/servers/{serverId:guid}/transport-activations")]
public sealed class ServerTransportActivationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServerTransportActivationsController(IMediator mediator)
        => _mediator = mediator;

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPost("{activationId:guid}/activate")]
    [ProducesResponseType(typeof(ServerTransportApplyJobAcceptedApiResponse), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<ServerTransportApplyJobAcceptedApiResponse>> ActivateTransportActivation(
        Guid serverId,
        Guid activationId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ActivateServerTransportRequest(serverId, activationId), ct);
        return Accepted(result.ToApiResponse());
    }

    [Authorize(Policy = AdminPolicies.ServersRead)]
    [HttpGet("~/api/admin/servers/{serverId:guid}/transport-apply-jobs/{jobId:guid}")]
    [ProducesResponseType(typeof(ServerTransportApplyJobStatusApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServerTransportApplyJobStatusApiResponse>> GetTransportApplyJob(
        Guid serverId,
        Guid jobId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetServerTransportApplyJobQuery(serverId, jobId), ct);
        return result is null ? NotFound() : Ok(result.ToApiResponse());
    }
    
    
    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AttachTransportActivations(
        Guid serverId,
        [FromBody] AttachServerTransportApiRequest body,
        CancellationToken ct = default)
    {
        await _mediator.Send(body.ToCommand(serverId), ct);
        return NoContent();
    }

    
    [Authorize(Policy = AdminPolicies.ServersRead)]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServerTransportApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServerTransportApiResponse>>> GetTransportActivations(
        Guid serverId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetServerTransportActivationsQuery(serverId), ct);
        return Ok(result.ToApiResponse());
    }

    
    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPut("{activationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTransportActivation(
        Guid serverId,
        Guid activationId,
        [FromBody] UpdateServerTransportApiRequest body,
        CancellationToken ct = default)
    {
        await _mediator.Send(body.ToCommand(serverId, activationId), ct);
        return NoContent();
    }
    
    
    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpDelete("{activationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTransportActivation(
        Guid serverId,
        Guid activationId,
        CancellationToken ct = default)
    {
        await _mediator.Send(new DeleteServerTransportActivationRequest(serverId, activationId), ct);
        return NoContent();
    }
}
