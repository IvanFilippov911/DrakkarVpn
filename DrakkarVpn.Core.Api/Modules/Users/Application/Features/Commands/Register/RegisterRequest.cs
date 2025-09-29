using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Shared.Users;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;

public sealed record RegisterRequest(RegisterUserRequest regCommand) : IRequest<RegisterUserResponse>;