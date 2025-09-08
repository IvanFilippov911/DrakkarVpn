using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Users.Commands;
using DrakkarVpn.Core.Api.Modules.Users.Application.Users.Queries;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Users.API;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _m;
    public UsersController(IMediator m) => _m = m;
    
    
    [HttpPost("register-or-get")]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public Task<AppUserDto> RegisterOrGet([FromBody] RegisterOrGetBody body, CancellationToken ct)
        => _m.Send(new RegisterOrGetByTelegram(body.TelegramId), ct);

    
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<AppUserDto> ChangeStatus([FromRoute] Guid id, [FromBody] ChangeStatusBody body, CancellationToken ct)
        => _m.Send(new ChangeStatus(id, body.Status), ct);

    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppUserDto>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var dto = await _m.Send(new GetUserById(id), ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    
    [HttpGet("by-tg/{telegramId:long}")]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AppUserDto>> GetByTelegramId([FromRoute] long telegramId, CancellationToken ct)
    {
        var dto = await _m.Send(new GetUserByTelegramId(telegramId), ct);
        return dto is null ? NotFound() : Ok(dto);
    }
    
    
    [HttpGet("all")]
    [ProducesResponseType(typeof(IReadOnlyList<AppUserDto>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<AppUserDto>> GetAll(CancellationToken ct) =>
        _m.Send(new GetAllUsers(), ct);

}

public sealed record RegisterOrGetBody(long TelegramId);
public sealed record ChangeStatusBody(UserStatus Status);