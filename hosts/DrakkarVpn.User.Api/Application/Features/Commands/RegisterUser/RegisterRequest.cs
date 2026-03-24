using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Shared.Users;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RegisterUser;

public sealed record RegisterRequest(RegisterUserRequest regCommand)
    : IRequest<RegisterUserResultDto>, IUsersCommand<RegisterUserResultDto>;