using DrakkarVpn.Core.Api.Modules.Admin.API.Auth;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Auth.Requests;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Auth.Responses;
using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LoginAdmin;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LogoutAdminSession;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.LogoutAllAdminSessions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Auth.RefreshAdminSession;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Auth.GetCurrentAdmin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Admin.Api.API.Controllers;

[ApiController]
[Route("api/admin/auth")]
public sealed class AdminAuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAdminAuthCookieService _cookieService;
    private readonly IAdminAuthRequestContextAccessor _requestContextAccessor;

    public AdminAuthController(
        IMediator mediator,
        IAdminAuthCookieService cookieService,
        IAdminAuthRequestContextAccessor requestContextAccessor)
    {
        _mediator = mediator;
        _cookieService = cookieService;
        _requestContextAccessor = requestContextAccessor;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AdminAuthApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminAuthApiResponse>> Login(
        [FromBody] AdminLoginApiRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new LoginAdminCommand(
                request.Email,
                request.Password,
                _requestContextAccessor.GetIpAddress(),
                _requestContextAccessor.GetUserAgent()),
            ct);

        _cookieService.SetRefreshToken(result.Tokens.RefreshToken, result.Tokens.RefreshTokenExpiresAtUtc);
        return Ok(result.ToResponse());
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AdminAuthApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminAuthApiResponse>> Refresh(CancellationToken ct)
    {
        var refreshToken = _cookieService.GetRequiredRefreshToken();

        var result = await _mediator.Send(
            new RefreshAdminSessionCommand(
                refreshToken,
                _requestContextAccessor.GetIpAddress(),
                _requestContextAccessor.GetUserAgent()),
            ct);

        _cookieService.SetRefreshToken(result.Tokens.RefreshToken, result.Tokens.RefreshTokenExpiresAtUtc);
        return Ok(result.ToResponse());
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (_cookieService.TryGetRefreshToken(out var refreshToken))
            await _mediator.Send(new LogoutAdminSessionCommand(refreshToken), ct);

        _cookieService.DeleteRefreshToken();
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAll(CancellationToken ct)
    {
        await _mediator.Send(new LogoutAllAdminSessionsCommand(), ct);
        _cookieService.DeleteRefreshToken();
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(AdminCurrentAdminApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminCurrentAdminApiResponse>> Me(CancellationToken ct)
    {
        var profile = await _mediator.Send(new GetCurrentAdminQuery(), ct);
        return Ok(profile.ToResponse());
    }
}
