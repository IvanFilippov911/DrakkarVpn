using DrakkarVpn.Admin.Api.API.Contracts.ServerTransportActivations;
using DrakkarVpn.Admin.Api.API.Mappings;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.ActivateServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.DeleteServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.UpdateServerTransportActivation;
using DrakkarVpn.Admin.Api.Application.Features.Queries.ServerTransportActivations.GetServerTransportActivations;
using DrakkarVpn.Admin.Api.Application.Features.Commands.ServerTransportActivations.AttachServerTransportActivations;
using DrakkarVpn.AdminAuth.Application.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Authorize]
[Route("api/admin/servers/{serverId:guid}/transport-activations")]
public sealed class ServerTransportActivationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServerTransportActivationsController(IMediator mediator)
        => _mediator = mediator;

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AttachTransportActivations(
        Guid serverId,
        [FromBody] AttachServerTransportActivationsApiRequest body,
        CancellationToken ct = default)
    {
        await _mediator.Send(body.ToCommand(serverId), ct);
        return NoContent();
    }

    [Authorize(Policy = AdminPolicies.ServersRead)]
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServerTransportActivationApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServerTransportActivationApiResponse>>> GetTransportActivations(
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
        [FromBody] UpdateServerTransportActivationApiRequest body,
        CancellationToken ct = default)
    {
        await _mediator.Send(body.ToCommand(serverId, activationId), ct);
        return NoContent();
    }

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPost("{activationId:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateTransportActivation(
        Guid serverId,
        Guid activationId,
        CancellationToken ct = default)
    {
        await _mediator.Send(new ActivateServerTransportRequest(serverId, activationId), ct);
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
