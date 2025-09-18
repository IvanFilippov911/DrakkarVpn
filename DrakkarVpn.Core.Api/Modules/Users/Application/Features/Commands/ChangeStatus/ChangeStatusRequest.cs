using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.ChangeStatus;

public sealed record ChangeStatusRequest(Guid UserId, UserStatus NewStatus) : IRequest<AppUserDto>;