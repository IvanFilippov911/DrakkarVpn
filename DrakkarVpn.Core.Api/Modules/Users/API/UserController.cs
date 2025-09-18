using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ChangeStatus;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetAllUsers;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserById;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserByTelegramId;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Users.API;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;
    
    
    public sealed record RegisterOrGetBody(long TelegramId);
    
    [HttpPost("register-or-get")]
    public Task<AppUserDto> RegisterOrGet([FromBody] RegisterOrGetBody body, CancellationToken ct)
        => _mediator.Send(new RegisterRequest(body.TelegramId), ct);

    
    public sealed record UserChangeStatusBody(UserStatus Status);
    
    [HttpPatch("{id:guid}/status")]
    public Task<AppUserDto> ChangeStatus([FromRoute] Guid id, [FromBody] UserChangeStatusBody body, CancellationToken ct)
        => _mediator.Send(new ChangeStatusRequest(id, body.Status), ct);

    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AppUserDto>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetUserByIdRequest(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    
    [HttpGet("by-tg/{telegramId:long}")]
    public async Task<ActionResult<AppUserDto>> GetByTelegramId([FromRoute] long telegramId, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetUserByTelegramIdRequest(telegramId), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
    
    
    [HttpGet("all")]
    public Task<IReadOnlyList<AppUserDto>> GetAll(CancellationToken ct) =>
        _mediator.Send(new GetAllUsersRequest(), ct);

}

