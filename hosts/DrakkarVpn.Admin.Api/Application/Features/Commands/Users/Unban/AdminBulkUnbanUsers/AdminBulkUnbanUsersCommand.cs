using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users.Bulk;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBulkUnbanUsers;

public sealed record AdminBulkUnbanUsersCommand(
    IReadOnlyCollection<Guid> UserIds
) : IRequest<BulkUsersOperationResponse>, IUsersCommand<BulkUsersOperationResponse>;