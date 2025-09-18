using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using MediatR;
namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetUserById;

public sealed record GetUserByIdRequest(Guid UserId) : IRequest<AppUserDto?>;