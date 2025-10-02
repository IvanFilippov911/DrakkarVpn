using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.CreateSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.DeleteSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.RenewSubscription;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Queries.GetActiveSubscriptionByUser;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.API;

[ApiController]
[Route("api/admin/subscriptions")]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SubscriptionsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateSubscriptionRequest body, CancellationToken ct)
    {
        var id = await _mediator.Send(body, ct);
        return CreatedAtAction(nameof(GetActiveByUser), new { userId = body.UserId }, id);
    }
    
    [HttpPost("{userId:guid}/renew")]
    public async Task<IActionResult> Renew(RenewSubscriptionRequest body, CancellationToken ct)
    {
        await _mediator.Send(body, ct);
        return NoContent();
    }
    
    [HttpGet("active/{userId:guid}")]
    public async Task<ActionResult<GetActiveSubscriptionDto>> GetActiveByUser(Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetActiveSubscriptionByUserRequest(userId), ct);
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteSubscriptionRequest(id), ct);
        return result ? Ok() : NotFound();
    }
}