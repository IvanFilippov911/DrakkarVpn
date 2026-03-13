using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminUserDetails;

public sealed record GetAdminUserDetailsQuery(Guid UserId)
    : IRequest<AdminUserDetailsDto>;