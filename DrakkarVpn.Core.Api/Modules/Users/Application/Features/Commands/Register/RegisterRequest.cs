using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;

public sealed record RegisterRequest(long TelegramId) : IRequest<AppUserDto>;