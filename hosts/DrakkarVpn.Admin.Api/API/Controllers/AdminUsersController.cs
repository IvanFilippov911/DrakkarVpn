using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBanUser;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBulkBanUsers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBulkMarkUsersInternal;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBulkUnbanUsers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminMarkUserInternal;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminUnbanUser;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServerUsers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminUserDetails;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Authorize]
[Route("api/admin")]
public sealed class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize(Policy = AdminPolicies.UsersRead)]
    [HttpGet("users")]
    [ProducesResponseType(typeof(PagedResponseDto<AdminUserCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkUsersOperationResponse>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] UserStatus? status = null,
        [FromQuery] SubscriptionStatus? subscriptionStatus = null,
        [FromQuery] UsersSortBy sortBy = UsersSortBy.CreatedAt,
        [FromQuery] UserSortDirection userSortDirection = UserSortDirection.Asc,
        
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetAdminUsersQuery(
            Page:               page,
            PageSize:           pageSize,
            Search:             search,
            Status:             status,
            SubscriptionStatus: subscriptionStatus,
            SortBy:             sortBy,
            UserSortDirection: userSortDirection
        ), ct);

        return Ok(result);
    }
    

    [Authorize(Policy = AdminPolicies.UsersManage)]
    [HttpPost("users/{userId:guid}/ban")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BanUser(
        Guid userId,
        [FromBody] BanUserRequest body,
        CancellationToken ct)
    {
        await _mediator.Send(
            new AdminBanUserCommand(userId, body.Reason),
            ct);

        return NoContent();
    }

    [Authorize(Policy = AdminPolicies.UsersManage)]
    [HttpPost("users/ban")]
    [ProducesResponseType(typeof(BulkUsersOperationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkUsersOperationResponse>> BulkBanUsers(
        [FromBody] BulkBanUsersRequest body,
        CancellationToken ct)
    {
        var res = await _mediator.Send(new AdminBulkBanUsersCommand(body.UserIds, body.Reason), ct);
        return Ok(res);
    }

    [Authorize(Policy = AdminPolicies.UsersManage)]
    [HttpPost("users/{userId:guid}/unban")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnbanUser(
        Guid userId,
        CancellationToken ct)
    {
        await _mediator.Send(
            new AdminUnbanUserCommand(userId),
            ct);

        return NoContent();
    }
    
    
    [Authorize(Policy = AdminPolicies.UsersManage)]
    [HttpPost("users/unban")]
    [ProducesResponseType(typeof(BulkUsersOperationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkUsersOperationResponse>> BulkUnbanUsers(
        [FromBody] BulkUnbanUsersRequest body,
        CancellationToken ct)
    {
        var res = await _mediator.Send(new AdminBulkUnbanUsersCommand(body.UserIds), ct);
        return Ok(res);
    }

    [Authorize(Policy = AdminPolicies.UsersManage)]
    [HttpPost("users/{userId:guid}/internal")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkUserInternal(
        [FromRoute] Guid userId,
        [FromBody] MarkUserInternalRequest body,
        CancellationToken ct)
    {
        var ok = await _mediator.Send(new AdminMarkUserInternalCommand(userId, body.IsInternal), ct);
        return ok ? NoContent() : NotFound();
    }

    [Authorize(Policy = AdminPolicies.UsersManage)]
    [HttpPost("users/internal")]
    [ProducesResponseType(typeof(BulkUsersOperationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkUsersOperationResponse>> BulkMarkUsersInternal(
        [FromBody] BulkMarkUsersInternalRequest body,
        CancellationToken ct)
    {
        var res = await _mediator.Send(new AdminBulkMarkUsersInternalCommand(body.UserIds, body.IsInternal), ct);
        return Ok(res);
    }
    
    
    [Authorize(Policy = AdminPolicies.UsersRead)]
    [HttpGet("users/{userId:guid}/details")]
    [ProducesResponseType(typeof(AdminUserDetailsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminUserDetailsDto>> GetUserDetails(
        [FromRoute] Guid userId,
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetAdminUserDetailsQuery(userId), ct);
        return Ok(dto);
    }
}