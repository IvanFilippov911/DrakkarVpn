using DrakkarVpn.Admin.Api.API.Contracts.TransportProfiles;
using DrakkarVpn.Admin.Api.API.Mappings;
using DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.CreateTransportProfile;
using DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.DeleteTransportProfile;
using DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.DisableTransportProfile;
using DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.EnableTransportProfile;
using DrakkarVpn.Admin.Api.Application.Features.Commands.TransportProfiles.UpdateTransportProfile;
using DrakkarVpn.Admin.Api.Application.Features.Queries.TransportProfiles.GetAdminTransportProfiles;
using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Servers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Authorize]
[Route("api/admin/transport-profiles")]
public sealed class AdminTransportProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminTransportProfilesController(IMediator mediator)
        => _mediator = mediator;

    [Authorize(Policy = AdminPolicies.ServersRead)]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<TransportProfileApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<TransportProfileApiResponse>>> GetTransportProfiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] bool? isEnabled = null,
        [FromQuery] TransportType? transportType = null,
        [FromQuery] TransportProfileSortByApi sortBy = TransportProfileSortByApi.GlobalPriority,
        [FromQuery] SortDirection sortDirection = SortDirection.Asc,
        CancellationToken ct = default)
    {
        var query = new GetAdminTransportProfilesQuery(
            Page: page,
            PageSize: pageSize,
            Search: search,
            IsEnabled: isEnabled,
            TransportType: transportType,
            SortBy: sortBy.ToDomainSortBy(),
            SortDirection: sortDirection);

        var result = await _mediator.Send(query, ct);
        return Ok(result.ToApiResponse());
    }

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPost]
    [ProducesResponseType(typeof(CreateTransportProfileApiResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateTransportProfileApiResponse>> CreateTransportProfile(
        [FromBody] CreateTransportProfileApiRequest body,
        CancellationToken ct = default)
    {
        var id = await _mediator.Send(body.ToCommand(), ct);
        return Created($"/api/admin/transport-profiles/{id}", new CreateTransportProfileApiResponse(id));
    }

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPut("{profileId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTransportProfile(
        Guid profileId,
        [FromBody] UpdateTransportProfileApiRequest body,
        CancellationToken ct = default)
    {
        var updated = await _mediator.Send(body.ToCommand(profileId), ct);
        return updated ? NoContent() : NotFound();
    }

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPost("{profileId:guid}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnableTransportProfile(Guid profileId, CancellationToken ct = default)
    {
        var updated = await _mediator.Send(new EnableTransportProfileRequest(profileId), ct);
        return updated ? NoContent() : NotFound();
    }

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPost("{profileId:guid}/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisableTransportProfile(Guid profileId, CancellationToken ct = default)
    {
        var updated = await _mediator.Send(new DisableTransportProfileRequest(profileId), ct);
        return updated ? NoContent() : NotFound();
    }

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpDelete("{profileId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteTransportProfile(Guid profileId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new DeleteTransportProfileRequest(profileId), ct);
        return result switch
        {
            DeleteTransportProfileResult.Deleted => NoContent(),
            DeleteTransportProfileResult.NotFound => NotFound(),
            DeleteTransportProfileResult.InUse => Conflict(),
            _ => Conflict()
        };
    }
}
