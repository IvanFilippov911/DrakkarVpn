using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Errors.DeleteCoreErrorEvent;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetCoreErrorEvents;
using DrakkarVpn.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Authorize(Policy = AdminPolicies.ObservabilityRead)]
[Route("admin/core/errors")]
public sealed class AdminCoreErrorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCoreErrorsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<PagedResponseDto<CoreErrorEventListItemDto>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? area = null,
        [FromQuery] string? errorType = null,
        [FromQuery] string? command = null,
        [FromQuery] string? domainCode = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? telegramId = null,
        [FromQuery] string? search = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null)
    {
        return await _mediator.Send(new GetCoreErrorEventsQuery(
            Page: page,
            PageSize: pageSize,
            Area: area,
            ErrorType: errorType,
            Command: command,
            DomainCode: domainCode,
            UserId: userId,
            TelegramId: telegramId,
            Search: search,
            FromUtc: fromUtc,
            ToUtc: toUtc
        ));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _mediator.Send(new DeleteCoreErrorEventCommand(id));
        return ok ? Ok() : NotFound();
    }
}