using DrakkarVpn.AdminAuth.Application.Contracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Auth.GetCurrentAdmin;

public sealed record GetCurrentAdminQuery : IRequest<AdminCurrentAdminProfile>;
