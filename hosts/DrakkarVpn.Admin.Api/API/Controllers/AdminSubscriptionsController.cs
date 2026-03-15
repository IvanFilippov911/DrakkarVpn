using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Request;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Subscriptions.Response;
using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminBulkCancelSubscriptions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminBulkGrantSubscriptions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminCancelSubscription;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminGrantSubscription;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Authorize(Policy = AdminPolicies.SubscriptionsManage)]
[Route("api/admin")]
public sealed class AdminSubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminSubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("users/{userId:guid}/subscriptions/grant")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SubscriptionDto>> GrantSubscription(
        [FromRoute] Guid userId,
        [FromBody] AdminGrantSubscriptionRequest body,
        CancellationToken ct)
    {
        var sub = await _mediator.Send(
            new AdminGrantSubscriptionCommand(userId, body.TariffId, body.DeviceCount),
            ct);

        return Ok(sub);
    }

    [HttpPost("users/subscriptions/grant")]
    [ProducesResponseType(typeof(BulkGrantSubscriptionsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkGrantSubscriptionsResponse>> BulkGrantSubscriptions(
        [FromBody] AdminBulkGrantSubscriptionsRequest body,
        CancellationToken ct)
    {
        var res = await _mediator.Send(
            new AdminBulkGrantSubscriptionsCommand(body.UserIds, body.TariffId, body.DeviceCount),
            ct);

        return Ok(res);
    }

    [HttpPost("subscriptions/{subscriptionId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSubscription(
        [FromRoute] Guid subscriptionId,
        CancellationToken ct)
    {
        await _mediator.Send(new AdminCancelSubscriptionCommand(subscriptionId), ct);
        return NoContent();
    }
    
    [HttpPost("subscriptions/cancel")]
    [ProducesResponseType(typeof(BulkCancelSubscriptionsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkCancelSubscriptionsResponse>> BulkCancelSubscriptions(
        [FromBody] BulkCancelSubscriptionsRequest body,
        CancellationToken ct)
    {
        var res = await _mediator.Send(new AdminBulkCancelSubscriptionsCommand(body.SubscriptionIds), ct);
        return Ok(res);
    }
}