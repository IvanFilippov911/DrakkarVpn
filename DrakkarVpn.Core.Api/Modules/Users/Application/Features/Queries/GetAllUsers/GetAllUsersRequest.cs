using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetAllUsers;

public sealed record GetAllUsersRequest : IRequest<IReadOnlyList<AppUserDto>>;