using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Users.Response;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs.Users.Bulk;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Users.AdminBulkBanUsers;

public sealed record AdminBulkBanUsersCommand(
    IReadOnlyList<Guid> UserIds,
    string? Reason
) : IRequest<BulkUsersOperationResponse>;