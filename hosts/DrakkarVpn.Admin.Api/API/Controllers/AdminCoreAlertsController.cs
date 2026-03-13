

using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.DeleteCoreAlert;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.ResolveCoreAlert;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreAlerts;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetGlobalCoreAlertsSummary;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Metadata;
using DrakkarVpn.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Route("api/admin/core/alerts")]
public sealed class AdminCoreAlertsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCoreAlertsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<CoreAlertListItemDto>), StatusCodes.Status200OK)]
    public async Task<PagedResponseDto<CoreAlertListItemDto>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool? isResolved = null,
        [FromQuery] string? source = null,
        [FromQuery] string? severity = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        CancellationToken ct = default)
    {
        return await _mediator.Send(
            new GetCoreAlertsQuery(page, pageSize, isResolved, source, severity, fromUtc, toUtc),
            ct);
    }
    
    [HttpGet("core/alerts/summary-global")]
    public async Task<ActionResult<CoreAlertsGlobalSummaryDto>> GetGlobalSummary(
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetGlobalCoreAlertsSummaryQuery(), ct);
        return Ok(dto);
    }
    
    [HttpGet("alerts/resolution-types")]
    public ActionResult<IReadOnlyList<CoreAlertResolutionTypeDto>> GetResolutionTypes()
    {
        return Ok(CoreAlertResolutionCatalog.All);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var ok = await _mediator.Send(new DeleteCoreAlertCommand(id), ct);
        return ok ? NoContent() : NotFound();
    }
    
    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(
        Guid id,
        [FromBody] ResolveCoreAlertRequest body,
        CancellationToken ct = default)
    {
        // TODO: вытаскивать текущего админа из auth-контекста
        Guid? adminId = null;

        var ok = await _mediator.Send(
            new ResolveCoreAlertCommand(
                Id:              id,
                ResolutionType:  body.ResolutionType,
                ResolutionNote:  body.ResolutionNote,
                ResolvedByAdminId: adminId),
            ct);

        return ok ? NoContent() : NotFound();
    }
    
}