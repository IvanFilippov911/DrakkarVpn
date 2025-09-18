using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserByTelegramId;

public sealed record GetUserByTelegramIdRequest(long TelegramId) : IRequest<AppUserDto?>;